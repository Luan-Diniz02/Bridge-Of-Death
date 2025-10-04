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
    private Camera mainCamera;

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 18f;

    [Header("Configurações de Faixa (Lane)")]
    [SerializeField] private float laneWidth = 3f;
    [SerializeField] private float laneChangeSpeed = 10f;

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
        mainCamera = Camera.main;
        initialPosition = playerTransform.position;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (characterController == null || mainCamera == null) return;

        Vector3 cameraForward = GetCameraForwardDirection();

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
        // --- CALCULA AS DIREÇÕES RELATIVAS À CÂMERA ---
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.x = 0;
        cameraRight.x = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        return cameraForward;
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
            int newLane = currentLane;
            
            if (input.x < 0) newLane--;
            else if (input.x > 0) newLane++;

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