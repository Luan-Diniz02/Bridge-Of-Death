using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Playables;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(LaneController))] 
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTES E REFERÊNCIAS ---
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    private CameraController cameraController;
    private LaneController laneController; 
    private AudioSource audioSource;

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 18f;
    [SerializeField] private bool allowMovement = true;

    // (Removido Header Configurações de Faixa - agora está no LaneController)

    [Header("Configurações de vida")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool dead = false;
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private bool isInvincible = false;

    // --- ESTADO INTERNO DO JOGADOR ---
    private float verticalVelocity;
    private Vector3 forwardVelocity;
    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocityVector;
    private Vector3 targetPosition;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<CameraController>();
        laneController = GetComponent<LaneController>(); 
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleMovement();
        StartCoroutine(increaseSpeedOverTime(0.1f, 1f));
    }

    private void HandleMovement()
    {
        if (characterController == null || cameraController == null) return;

        bool flowControl = CheckMovementAllowed();
        if (!flowControl) return; // Se não pode mover, interrompe aqui ou ajusta lógica

        Vector3 cameraForward = cameraController.GetCameraForwardDirection();

        // --- 1. MOVIMENTO PARA FRENTE ---
        forwardVelocity = cameraForward * speed;

        // --- 2. MOVIMENTO HORIZONTAL (Via LaneController) ---
        // Perguntamos ao LaneController qual a velocidade horizontal necessária agora
        horizontalVelocity = laneController.CalculateLaneMovement(transform.position);

        // --- 3. GRAVIDADE E PULO ---
        UpdateGravityAndJump();
        verticalVelocityVector = Vector3.up * verticalVelocity;

        // --- 4. COMBINA TODOS OS MOVIMENTOS ---
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

        // Input Vertical (Pulo / Deslize)
        if (input.y > 0f) Jump();
        else if (input.y < 0f) Slider();

        // Input Horizontal (Troca de Faixa)
        if (Mathf.Abs(input.x) > 0f && !laneController.IsChangingLane)
        {
            Vector3 cameraRight = cameraController.GetCameraRightDirection();
            
            // Lógica de direção baseada na câmera permanece aqui, pois é input do jogador
            float dotProduct = Vector3.Dot(cameraRight, Vector3.right);
            int direction = 0;

            if (input.x < 0)
            {
                direction = (dotProduct > 0) ? -1 : 1;
            }
            else if (input.x > 0)
            {
                direction = (dotProduct > 0) ? 1 : -1;
            }

            // Manda o comando final para o LaneController
            if (direction != 0)
            {
                laneController.MoveLane(direction);
            }
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
        if (isInvincible || dead) return;
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
    
    // Trigger Enter permanece igual
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            DamagePlayer();
            Destroy(other.gameObject);
        }
    }
}