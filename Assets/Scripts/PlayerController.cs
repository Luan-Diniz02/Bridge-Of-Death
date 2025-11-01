using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CameraController))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTES E REFERÊNCIAS ---
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    private Transform playerTransform;
    private CameraController cameraController;

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 18f;
    [SerializeField] private bool allowMovement = true;

    [Header("Configurações de Faixa (Lane)")]
    [SerializeField] private float laneWidth = 3f;
    [SerializeField] private float laneChangeSpeed = 10f;

    [Header("Configurações de vida")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool dead = false;

    // --- ESTADO INTERNO DO JOGADOR ---
    private int currentLane = 0; // -1: Esquerda, 0: Centro, 1: Direita
    private float verticalVelocity;
    private Vector3 initialPosition;
    private Vector3 forwardVelocity;
    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocityVector;
    private Vector3 targetPosition;
    private bool isChangingLane = false;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        cameraController = GetComponent<CameraController>();

        playerTransform = transform;
        initialPosition = playerTransform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (characterController == null || cameraController == null) return;

        Vector3 cameraForward = cameraController.GetCameraForwardDirection();
        bool flowControl = CheckMovementAllowed();

        // --- MOVIMENTO PARA FRENTE ---
        forwardVelocity = cameraForward * speed;

        UpdateLanePosition();
        UpdateGravityAndJump();

        verticalVelocityVector = Vector3.up * verticalVelocity;

        if (!flowControl) return;
 
        // --- COMBINA TODOS OS MOVIMENTOS EM UMA ÚNICA CHAMADA ---
        targetPosition = forwardVelocity + horizontalVelocity + verticalVelocityVector;
        characterController.Move(targetPosition * Time.deltaTime);
    }

    private bool CheckMovementAllowed()
    {
        if (!allowMovement || dead)
        {
            animator.SetFloat("Speed", 0);
            return false;
        }
        else
        {
            animator.SetFloat("Speed", 1);
        }

        return true;
    }

    private void UpdateGravityAndJump()
    {
        // --- GRAVIDADE E PULO ---
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    private void UpdateLanePosition()
    {
        // --- MOVIMENTO HORIZONTAL (MUDANÇA DE FAIXA) ---
        if (isChangingLane)
        {
            Vector3 targetLanePosition = initialPosition;
            targetLanePosition.x += currentLane * laneWidth;

            float currentX = playerTransform.position.x;
            float targetX = targetLanePosition.x;

            // Calcula a velocidade horizontal necessária para chegar à faixa alvo
            float horizontalSpeed = (targetX - currentX) * laneChangeSpeed;
            horizontalVelocity = Vector3.right * horizontalSpeed;

            // Para o movimento quando chegar próximo da posição target
            if (Mathf.Abs(currentX - targetX) < 0.1f)
            {
                horizontalVelocity = Vector3.zero;
                isChangingLane = false;
            }
        }
        else
        {
            horizontalVelocity = Vector3.zero;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 input = context.ReadValue<Vector2>();

        // Input Vertical (Pulo / Deslize)
        if (input.y > 0f) Jump();
        else if (input.y < 0f) Slider();

        // Input Horizontal (Troca de Faixa)
        if (Mathf.Abs(input.x) > 0f && !isChangingLane)
        {
            Vector3 cameraRight = cameraController.GetCameraRightDirection();
            
            int newLane = currentLane;
            
            // Determina a direção baseada na orientação da câmera
            float dotProduct = Vector3.Dot(cameraRight, Vector3.right);
            
            if (input.x < 0)
            {
                newLane += (dotProduct > 0) ? -1 : 1;
            }
            else if (input.x > 0)
            {
                newLane += (dotProduct > 0) ? 1 : -1;
            }

            newLane = Mathf.Clamp(newLane, -1, 1);
            
            if (newLane != currentLane)
            {
                currentLane = newLane;
                isChangingLane = true;
            }
        }
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    private void Slider()
    {
        if (animator != null) animator.SetTrigger("Slider");
    }

    public void DamagePlayer()
    {
        currentHealth--;
        Debug.Log("Player Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            dead = true;
            animator.SetTrigger("Dead");
            DisableInput();
            cameraController.ActivateDeathCamera();
        }
        else{
            animator.SetTrigger("Hit");
        }
    }

    public void DisableInput() {
        if (playerInput != null) {
            playerInput.DeactivateInput();
        }
    }

    public void EnableInput() {
        if (playerInput != null) {
            playerInput.ActivateInput();
        }
    }
    public void DisableInputTemporarily(float duration)
    {
        DisableInput();
        Invoke(nameof(EnableInput), duration);
    }
}