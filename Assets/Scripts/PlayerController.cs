using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(CharacterController))]

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour

{
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Animator animator;
    private Transform playerTransform;
    [SerializeField] private float speed = 5f, jumpForce = 5f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        // Verificar se componentes são válidos
        if (characterController == null) return;

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

        MoveFoward(velocity);

    }

    private void MoveFoward(Vector3 velocity)
    {
        // Aplica o movimento de corrida 
        Vector3 finalMovement = (Vector3.right * speed + velocity) * Time.deltaTime;
        characterController.Move(finalMovement);
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

    private void Jump()
    {
        characterController.Move(jumpForce * Time.deltaTime * Vector3.up);
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }

    private void Slider()
    {
        if (animator != null)
        {
            animator.SetTrigger("Slider");
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