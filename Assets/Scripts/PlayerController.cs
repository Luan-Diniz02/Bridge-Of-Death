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

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (characterController == null || mainCamera == null) return;

        // --- CALCULA AS DIREÇÕES RELATIVAS À CÂMERA ---
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // --- MOVIMENTO PARA FRENTE ---
        forwardVelocity = cameraForward * speed;

        // --- GRAVIDADE E PULO ---
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        
        verticalVelocityVector = Vector3.up * verticalVelocity;

        targetPosition = forwardVelocity + verticalVelocityVector;

        characterController.Move(targetPosition * Time.deltaTime);

        // Sempre chama MoveLane para continuar o movimento
        if (isChangingLane)
        {
            MoveLane();
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
            animator?.SetTrigger("Jump");
        }
    }

    private void Slider()
    {
        animator?.SetTrigger("Slider");
    }

    private void MoveLane()
    {
        Vector3 targetLanePosition = initialPosition;
        targetLanePosition.x += currentLane * laneWidth;
        
        Vector3 currentPos = playerTransform.position;
        currentPos.x = Mathf.MoveTowards(currentPos.x, targetLanePosition.x, laneChangeSpeed * Time.deltaTime);
        
        playerTransform.position = new Vector3(currentPos.x, playerTransform.position.y, playerTransform.position.z);
        
        // Para o movimento quando chegar na posição target
        if (Mathf.Approximately(currentPos.x, targetLanePosition.x))
        {
            isChangingLane = false;
        }
    }

    public void DisableInput() => playerInput?.DeactivateInput();
    public void EnableInput() => playerInput?.ActivateInput();
    public void DisableInputTemporarily(float duration)
    {
        DisableInput();
        Invoke(nameof(EnableInput), duration);
    }
}