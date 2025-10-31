using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTES E REFERÊNCIAS ---
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    private Transform playerTransform;
    [SerializeField] private Camera mainCamera;

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
        playerTransform = transform;
        if(mainCamera == null) {
            mainCamera = Camera.main;
        }
        initialPosition = playerTransform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (characterController == null || mainCamera == null) return;

        Vector3 cameraForward = GetCameraForwardDirection();

        if (!allowMovement) return;
        // --- MOVIMENTO PARA FRENTE ---
        forwardVelocity = cameraForward * speed;

        UpdateLanePosition();
        UpdateGravityAndJump();

        verticalVelocityVector = Vector3.up * verticalVelocity;

        // --- COMBINA TODOS OS MOVIMENTOS EM UMA ÚNICA CHAMADA ---
        targetPosition = forwardVelocity + horizontalVelocity + verticalVelocityVector;
        characterController.Move(targetPosition * Time.deltaTime);
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

    private Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        return cameraForward;
    }

    private Vector3 GetCameraRightDirection()
    {
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();
        return cameraRight;
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
            Vector3 cameraRight = GetCameraRightDirection();
            
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
        if (currentHealth <= 0)
        {
            dead = true;
            animator.SetTrigger("Dead");
            DisableInput();
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