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
    [SerializeField] private float laneWidth = 3f; // Distância entre as faixas
    [SerializeField] private float laneChangeSpeed = 10f; // Velocidade com que o personagem corrige a rota para a faixa

    // --- ESTADO INTERNO DO JOGADOR ---
    private int currentLane = 0; // -1: Esquerda, 0: Centro, 1: Direita
    private float verticalVelocity;
    
    // Ponto de referência que se move sempre para frente, representando o centro do caminho
    private Vector3 centerPathPoint;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerTransform = transform;
        mainCamera = Camera.main;
    }

    void Start()
    {
        // Garante que a câmera seja encontrada mesmo que a tag não esteja definida no Awake
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        // Inicializa o ponto de referência do caminho na posição inicial do personagem
        centerPathPoint = playerTransform.position;

        // Opcional: Detecta a faixa inicial com base na posição no editor
        // Isso evita o "salto" inicial se o personagem não começar no centro.
        if (laneWidth > 0 && mainCamera != null)
        {
            Vector3 relativePos = playerTransform.position - centerPathPoint;
            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();
            
            // Calcula o quanto o personagem está para o lado do ponto central
            float sideOffset = Vector3.Dot(relativePos, cameraRight);
            currentLane = Mathf.RoundToInt(sideOffset / laneWidth);
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
        // Zera o Y para que o movimento seja sempre no plano horizontal
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // --- MOVIMENTO PARA FRENTE ---
        Vector3 forwardVelocity = cameraForward * speed;

        // Atualiza o ponto de referência para que ele se mova sempre para frente
        centerPathPoint += forwardVelocity * Time.deltaTime;

        // --- GRAVIDADE E PULO ---
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Mantém no chão
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        Vector3 verticalMovement = Vector3.up * verticalVelocity;

        // --- MOVIMENTO LATERAL (CORREÇÃO DE FAIXA) ---
        // Calcula a posição alvo exata onde o personagem deveria estar
        Vector3 targetPosition = centerPathPoint + (currentLane * laneWidth * cameraRight);

        // Calcula o vetor necessário para ir da posição atual para a posição alvo
        Vector3 correctionVector = targetPosition - playerTransform.position;
        // Ignora a diferença de altura (Y), pois ela é controlada pela gravidade/pulo
        correctionVector.y = 0;
        
        // Multiplicamos pela velocidade de troca de faixa para controlar a suavidade
        Vector3 lateralVelocity = correctionVector * laneChangeSpeed;

        // --- COMBINA TODOS OS VETORES DE MOVIMENTO ---
        Vector3 finalMovement = forwardVelocity + verticalMovement + lateralVelocity;
        
        // Aplica o movimento final em uma única chamada
        characterController.Move(finalMovement * Time.deltaTime);
    }
    
    // Unifiquei seus inputs em um único método para simplificar
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 input = context.ReadValue<Vector2>();

        // Input Vertical (Pulo / Deslize)
        if (input.y > 0.5f) Jump();
        else if (input.y < -0.5f) Slider();

        // Input Horizontal (Troca de Faixa)
        if (Mathf.Abs(input.x) > 0.5f)
        {
            int previousLane = currentLane;
            if (input.x < 0) currentLane--;
            else if (input.x > 0) currentLane++;
            
            currentLane = Mathf.Clamp(currentLane, -1, 1);

            // Quando trocamos de faixa, ajustamos a referência central para a posição atual do jogador
            // Isso evita que o personagem "deslize" para trás ou para frente ao trocar de faixa em uma curva
            if (currentLane != previousLane)
            {
                 centerPathPoint = playerTransform.position;
                 Vector3 relativePos = playerTransform.position - centerPathPoint;
                 Vector3 cameraRight = mainCamera.transform.right;
                 cameraRight.y = 0;
                 cameraRight.Normalize();
                 float sideOffset = Vector3.Dot(relativePos, cameraRight);
                 centerPathPoint -= cameraRight * sideOffset;
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
    
    // Os métodos de desabilitar input continuam os mesmos
    public void DisableInput() => playerInput?.DeactivateInput();
    public void EnableInput() => playerInput?.ActivateInput();
    public void DisableInputTemporarily(float duration)
    {
        DisableInput();
        Invoke(nameof(EnableInput), duration);
    }
}