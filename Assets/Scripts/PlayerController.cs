using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Playables;
using System;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(LaneController))] 
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    private CameraController cameraController;
    private LaneController laneController; 
    private AudioSource audioSource;
    
    [Header("UI Power-Ups")]
    [SerializeField] private BarManager speedBoostBar;
    [SerializeField] private BarManager shieldBar;

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 18f;
    [SerializeField] private bool allowMovement = true;

    [Header("Configurações de vida")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool dead = false;
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private bool isInvincible = false;
    private bool shieldActive = false;

    private float verticalVelocity;
    private Vector3 forwardVelocity;
    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocityVector;
    private Vector3 targetPosition;
    private float baseSpeed;
    private float speedMultiplier = 1f; 
    private bool hasSpeedBoost = false;
    
    [Header("Incremento de Velocidade")]
    [SerializeField] private float speedIncreaseAmount = 0.1f;
    [SerializeField] private float speedIncreaseInterval = 1f;
    private float nextSpeedIncreaseTime;

    // Variáveis para controlar as Corrotinas ativas
    private Coroutine speedBoostCoroutine;
    private Coroutine shieldCoroutine;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<CameraController>();
        laneController = GetComponent<LaneController>(); 
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        baseSpeed = speed;
        nextSpeedIncreaseTime = Time.time + speedIncreaseInterval;
    }

    void Update()
    {
        HandleMovement();
        UpdateSpeedIncrease();
    }

    private void HandleMovement()
    {
        if (characterController == null || cameraController == null) return;

        bool flowControl = CheckMovementAllowed();
        if (!flowControl) return; 

        Vector3 cameraForward = cameraController.GetCameraForwardDirection();

        float finalSpeed = baseSpeed * speedMultiplier;
        forwardVelocity = cameraForward * finalSpeed;

        horizontalVelocity = laneController.CalculateLaneMovement(transform.position);

        UpdateGravityAndJump();
        verticalVelocityVector = Vector3.up * verticalVelocity;

        targetPosition = forwardVelocity + horizontalVelocity + verticalVelocityVector;
        characterController.Move(targetPosition * Time.deltaTime);
    }

    private bool CheckMovementAllowed()
    {
        if (!allowMovement || dead)
        {
            animator.SetFloat("Speed", 0);
            if(audioSource != null && audioSource.isPlaying) audioSource.Stop();
            return false;
        }
        else
        {
            animator.SetFloat("Speed", 1);
            if(audioSource != null && !audioSource.isPlaying) audioSource.Play();
        }
        return true;
    }

    private void UpdateGravityAndJump()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed || dead || !allowMovement) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.y > 0f) Jump();
        else if (input.y < 0f) Slider();

        if (Mathf.Abs(input.x) > 0f && !laneController.IsChangingLane)
        {
            Vector3 cameraRight = cameraController.GetCameraRightDirection();
            
            float dotProduct = Vector3.Dot(cameraRight, Vector3.right);
            int direction = 0;

            if (input.x < 0) direction = (dotProduct > 0) ? -1 : 1;
            else if (input.x > 0) direction = (dotProduct > 0) ? 1 : -1;

            if (direction != 0) laneController.MoveLane(direction);
        }
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            if (animator != null) animator.SetTrigger("Jump");
        }
    }

    private void Slider()
    {
        if (animator != null) animator.SetTrigger("Slider");
    }

    public void DamagePlayer()
    {
        if (isInvincible || dead || shieldActive) return;
        currentHealth--;
        PlayerStats.Instance.TakeDamage(1); 
        
        if (currentHealth <= 0)
        {
            dead = true;
            animator.SetTrigger("Dead");
            DisableInput();
            cameraController.ActivateDeathCamera();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
        StartCoroutine(InvincibilityCoroutine());
    }

    public void HealPlayer(int amount)
    {
        if (dead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        PlayerStats.Instance.Heal(amount); 
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private IEnumerator increaseSpeedOverTime(float amount, float duration)
    {
        float elapsed = 0f;
        float initialSpeed = speed;
        float targetSpeed = initialSpeed + amount;

        while (elapsed < duration)
        {
            speed = Mathf.Lerp(initialSpeed, targetSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        speed = targetSpeed;
    }

    public void DisableInput() {
        if (playerInput != null) playerInput.DeactivateInput();
    }

    public void EnableInput() {
        if (playerInput != null) playerInput.ActivateInput();
    }

    // --- LÓGICA DE SPEED BOOST CORRIGIDA ---
    public void ApplySpeedBoost(float duration, float effectValue)
    {
        // Se já existe uma corrotina rodando, para ela IMEDIATAMENTE
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        if (hasSpeedBoost)
        {
            // Se já tem o boost, apenas reseta o timer visual com a nova duração
            if (speedBoostBar != null) speedBoostBar.ResetTimer(duration);
        }
        else
        {
            // Se não tem, inicia a barra do zero
            if (speedBoostBar != null) speedBoostBar.StartTimer(duration);
        }

        // Inicia a nova corrotina e guarda a referência
        speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine(duration, effectValue));
    }

    private IEnumerator SpeedBoostCoroutine(float duration, float effectValue)
    {
        hasSpeedBoost = true;
        speedMultiplier = effectValue;
        
        yield return new WaitForSeconds(duration);
        
        speedMultiplier = 1f;
        hasSpeedBoost = false;
        speedBoostCoroutine = null; // Limpa a referência
        
        if (speedBoostBar != null) speedBoostBar.StopTimer();
    }
    
    private void UpdateSpeedIncrease()
    {
        if (dead || !allowMovement) return;
        
        if (Time.time >= nextSpeedIncreaseTime)
        {
            baseSpeed += speedIncreaseAmount;
            nextSpeedIncreaseTime = Time.time + speedIncreaseInterval;
        }
    }

    // --- LÓGICA DE SHIELD CORRIGIDA ---
    public void ApplyShield(float duration)
    {
        // Para a corrotina anterior se existir
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }

        if (shieldActive)
        {
            if (shieldBar != null) shieldBar.ResetTimer(duration);
        }
        else
        {
            if (shieldBar != null) shieldBar.StartTimer(duration);
        }
        
        // Inicia e guarda referência
        shieldCoroutine = StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        shieldActive = true;
        Debug.Log("Shield ativado!");
        
        yield return new WaitForSeconds(duration);
        
        shieldActive = false;
        shieldCoroutine = null; // Limpa a referência
        Debug.Log("Shield desativado!");
        
        if (shieldBar != null) shieldBar.StopTimer();
    }

    public bool getIsShield()
    {
        return shieldActive;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}