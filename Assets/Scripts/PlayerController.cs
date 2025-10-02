using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // MODIFICAÇÃO: Agora a movimentação é relativa à câmera principal
    // - O movimento para frente segue a direção da câmera
    // - O movimento lateral (faixas) é calculado baseado na orientação da câmera
    // - Fallback para movimento original caso a câmera não seja encontrada
    // --- COMPONENTES E VARIÁVEIS PRINCIPAIS ---
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    private Transform playerTransform;
    private Camera mainCamera;

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f; // Ajuste este valor para a altura do pulo
    [SerializeField] private float gravity = 18f; // Um valor um pouco maior que 9.81 para um pulo mais "arcade"

    // --- LÓGICA DE MOVIMENTO LATERAL (FAIXAS) ---
    [Header("Configurações de Faixa (Lane)")]
    [SerializeField] private float laneWidth = 3f; // Distância entre o centro e uma faixa lateral
    [SerializeField] private float laneChangeSpeed = 15f; // Velocidade da transição entre as faixas

    // Variáveis internas para controlar o estado do jogador
    private int currentLane = 0; // -1: Esquerda, 0: Centro, 1: Direita
    private float verticalVelocity; // Guarda a velocidade vertical para pulo e gravidade
    private Vector3 targetPosition; // Posição alvo para o movimento lateral suave
    private Vector3 centerLanePosition; // Posição de referência para a faixa central
    private bool isTransitioningLanes = false; // Controla se está em transição entre faixas

    void Awake()
    {
        // Pega as referências dos componentes no início do jogo
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerTransform = transform;
        mainCamera = Camera.main;
        
        // Se Camera.main não funcionar, tenta encontrar a primeira câmera ativa na cena
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        // Força o personagem a começar na faixa central (currentLane = 0)
        currentLane = 0;
        
        // Inicializa a posição alvo e central com a posição inicial do personagem
        targetPosition = playerTransform.position;
        centerLanePosition = playerTransform.position;
        isTransitioningLanes = false;
    }

    void Start()
    {
        // Dupla verificação para garantir que a câmera foi encontrada
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
        }
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (characterController == null) return;

        // --- MOVIMENTO PARA FRENTE (Relativo à câmera) ---
        Vector3 forwardMovement = Vector3.zero;
        if (mainCamera != null)
        {
            // Pega a direção para frente da câmera (ignorando a componente Y para manter o movimento no plano horizontal)
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            
            forwardMovement = cameraForward * speed;
        }
        else
        {
            // Fallback para o movimento original caso a câmera não seja encontrada
            forwardMovement = Vector3.right * speed;
        }

        // --- GRAVIDADE E PULO (MOVIMENTO VERTICAL) ---
        // Verifica se o personagem está no chão
        bool isGrounded = characterController.isGrounded;
        if (isGrounded)
        {
            // Se estiver no chão e caindo, reseta a velocidade vertical
            if (verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Uma pequena força para baixo para manter o personagem "colado"
            }
        }
        else
        {
            // Se estiver no ar, aplica a gravidade continuamente
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // --- COMBINA OS MOVIMENTOS ---
        // Cria o vetor de movimento final combinando o avanço e a velocidade vertical
        Vector3 finalMovement = forwardMovement;
        finalMovement.y = verticalVelocity;

        // Aplica o movimento para frente e a gravidade usando o CharacterController
        characterController.Move(finalMovement * Time.deltaTime);

        // --- MOVIMENTO LATERAL (Relativo à câmera) ---
        HandleLateralMovement();
    }

    private void HandleLateralMovement()
    {
        // Só aplica movimento lateral se estiver em transição entre faixas
        if (!isTransitioningLanes) return;

        if (mainCamera != null)
        {
            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            // Calcula a posição alvo da faixa baseada na direção da câmera
            Vector3 laneOffset = cameraRight * (currentLane * laneWidth);
            targetPosition = centerLanePosition + laneOffset;
            targetPosition.y = playerTransform.position.y;
            
            // Move apenas no eixo lateral (direção da câmera)
            Vector3 currentPos = playerTransform.position;
            Vector3 currentLateralPos = Vector3.Project(currentPos - centerLanePosition, cameraRight) + centerLanePosition;
            Vector3 newLateralPos = Vector3.Lerp(currentLateralPos, targetPosition, Time.deltaTime * laneChangeSpeed);
            
            // Aplica apenas o componente lateral, preservando movimento para frente
            Vector3 lateralDifference = newLateralPos - currentLateralPos;
            playerTransform.position += lateralDifference;
            
            // Verifica se chegou próximo o suficiente da posição alvo para parar a transição
            float distanceToTarget = Vector3.Distance(Vector3.Project(playerTransform.position - centerLanePosition, cameraRight), Vector3.Project(targetPosition - centerLanePosition, cameraRight));
            if (distanceToTarget < 0.1f)
            {
                isTransitioningLanes = false;
            }
        }
        else
        {
            // Fallback para o movimento original caso a câmera não seja encontrada
            targetPosition.z = currentLane * laneWidth;
            targetPosition.x = playerTransform.position.x;
            targetPosition.y = playerTransform.position.y;
            
            playerTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, Time.deltaTime * laneChangeSpeed);
        }
    }

    // --- FUNÇÕES CHAMADAS PELO PLAYER INPUT ---

    public void OnMoveHorizontal(InputAction.CallbackContext context)
    {
        // Esta função deve ser chamada quando o jogador pressionar as teclas de movimento lateral (A/D, Setas)
        if (context.performed)
        {
            // Armazena a faixa anterior para comparação
            int previousLane = currentLane;
            
            // Pega a direção do input (-1 para esquerda, 1 para direita)
            float direction = context.ReadValue<Vector2>().x;

            if (direction < 0)
            {
                currentLane--; // Move para a faixa da esquerda
            }
            else if (direction > 0)
            {
                currentLane++; // Move para a faixa da direita
            }

            // Garante que o valor de 'currentLane' fique sempre entre -1 e 1
            currentLane = Mathf.Clamp(currentLane, -1, 1);
            
            // Só inicia transição se realmente mudou de faixa
            if (currentLane != previousLane)
            {
                // Atualiza a posição central baseada na posição atual do personagem
                centerLanePosition = playerTransform.position;
                isTransitioningLanes = true;
            }
        }
    }

    public void OnMoveVertical(InputAction.CallbackContext context)
    {
        float direction = context.ReadValue<Vector2>().y;
        if (context.performed)
        {
            if (direction > 0)
            {
                Debug.Log("Up action triggered");
                Jump();
            }
            else if (direction < 0)
            {
                Debug.Log("Down action triggered");
                Slider();
            }
        }
    }

    // --- LÓGICA DAS AÇÕES ---

    private void Jump()
    {
        // O personagem só pode pular se estiver no chão
        if (characterController.isGrounded)
        {
            // Define a velocidade vertical para a força do pulo, iniciando o movimento para cima
            verticalVelocity = jumpForce;

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    private void Slider()
    {
        // TODO: Além da animação, você pode querer reduzir a altura do CharacterController aqui
        if (animator != null)
        {
            animator.SetTrigger("Slider");
        }
    }

    // --- MÉTODOS PARA CONTROLE DO INPUT ---
    public void DisableInput()
    {
        if (playerInput != null)
        {
            playerInput.DeactivateInput();
        }
    }

    public void EnableInput()
    {
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }
    }

    public void DisableInputTemporarily(float duration)
    {
        DisableInput();
        Invoke(nameof(EnableInput), duration);
    }
}