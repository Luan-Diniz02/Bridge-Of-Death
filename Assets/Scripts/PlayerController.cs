using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(CharacterController))]

[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour

{
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    [SerializeField] private float speed = 5f, rotationSpeed = 5f;
    private Vector2 moveInput;
    private Camera mainCamera;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //HandleMovement();
    }
    private void HandleMovement()
    {
        // Verificar se componentes são válidos
        if (characterController == null || mainCamera == null) return;

        // Verificar se está no chão
        bool isGrounded = characterController.isGrounded;

        // Aplicar gravidade
        Vector3 velocity = Vector3.zero;
        if (!isGrounded)
        {
            velocity.y = -9.81f; // Gravidade simples
        }
        else if (velocity.y < 0f)
        {
            velocity.y = -2f; // Força para manter no chão
        }

        // Criar vetor de movimento base 
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
        float inputMagnitude = movement.magnitude;

        if (inputMagnitude > 0.1f) // Threshold mínimo para detectar movimento
        {
            // Transformar direção usando a câmera
            if (mainCamera != null)
            {
                movement = mainCamera.transform.TransformDirection(movement);
                movement.y = 0f; // Manter movimento horizontal
                movement.Normalize(); // Normalizar o vetor
            }

            // Rotação suave 
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movement),
                rotationSpeed * Time.deltaTime
            );
        }

        // Aplicar movimento final 
        Vector3 finalMovement = (movement * speed + velocity) * Time.deltaTime;
        characterController.Move(finalMovement);

        // Atualizar animação baseada na magnitude do input original
        float animationSpeed = moveInput.magnitude;
        animator.SetFloat("Speed", animationSpeed);
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // Armazenar o input para usar no Update
        moveInput = context.ReadValue<Vector2>();
    }

    public void SetSittingAnimation(bool isSitting)
    {
        if (animator != null)
        {
            animator.SetBool("Sitting", isSitting);
        }
    }

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