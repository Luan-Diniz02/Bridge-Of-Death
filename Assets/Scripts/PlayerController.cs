using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTES E VARIÁVEIS PRINCIPAIS ---
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    private Transform playerTransform;

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


    void Awake()
    {
        // Pega as referências dos componentes no início do jogo
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerTransform = transform;

            if (laneWidth > 0) // Evita uma possível divisão por zero
                {
                    currentLane = Mathf.RoundToInt(playerTransform.position.z / laneWidth);
                }
            else
                {
                    currentLane = 0;
                }
                

        // Inicializa a posição alvo com a posição inicial do personagem
        targetPosition = playerTransform.position;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (characterController == null) return;

        // --- MOVIMENTO PARA FRENTE (EIXO X, de acordo com sua gambiarra) ---
        Vector3 forwardMovement = Vector3.right * speed;

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

        // --- MOVIMENTO LATERAL (EIXO Z) ---
        // Calcula a posição Z alvo baseada na faixa atual (currentLane)
        // A posição X e Y vem da posição atual do transform para não interferir com os outros movimentos
        targetPosition.z = currentLane * laneWidth;
        targetPosition.x = playerTransform.position.x;
        targetPosition.y = playerTransform.position.y;


        // Usa Lerp (Interpolação Linear) para mover suavemente a posição do personagem para a faixa alvo
        // Isso cria a transição suave em vez de um "teleporte" instantâneo
        playerTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, Time.deltaTime * laneChangeSpeed);
    }

    // --- FUNÇÕES CHAMADAS PELO PLAYER INPUT ---

    public void OnMoveHorizontal(InputAction.CallbackContext context)
    {
        // Esta função deve ser chamada quando o jogador pressionar as teclas de movimento lateral (A/D, Setas)
        if (context.performed)
        {
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump action triggered");
            Jump();
        }
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Slide action triggered");
            Slider();
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