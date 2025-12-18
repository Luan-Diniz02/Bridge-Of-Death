using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LaneController))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    // [SerializeField] private GameObject player; // REMOVIDO - agora usa PlayerManager
    [SerializeField] private float speedEnemy = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float timeToDestroy = 5f;
    
    [Header("Configurações de Pulo")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 18f;
    
    [Header("Configurações de Mudança de Faixa")]
    [SerializeField] private float minLaneChangeInterval = 2f;
    [SerializeField] private float maxLaneChangeInterval = 5f;

    [Header("Sons")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f; 
    [SerializeField] private float knockbackDrag = 2f;   
    
    [SerializeField] private UnityEvent onEnemyDeath;
    
    private Transform playerTransform;
    private Transform enemyTransform;
    private Animator enemyAnimator;    
    private float distanceToPlayer;
    private PlayerController playerController;
    private Collider enemyCollider;
    private bool hasNearbyPlayer = false;
    private LaneController laneController;
    private CharacterController characterController;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float nextLaneChangeTime;
    
    private bool isDead = false;

    void Awake()
    {
        enemyTransform = transform;
        enemyAnimator = enemyTransform.GetComponent<Animator>();            
        enemyCollider = GetComponent<Collider>();
        laneController = GetComponent<LaneController>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        ScheduleNextLaneChange();
    }
    
    void Start()
    {
        // Encontra o player mais próximo através do PlayerManager
        FindTargetPlayer();
    }
    
    private void FindTargetPlayer()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogWarning("PlayerManager não encontrado! Certifique-se de ter um PlayerManager na cena.");
            return;
        }
        
        // Pega o player mais próximo
        playerController = PlayerManager.Instance.GetClosestPlayer(transform.position);
        
        if (playerController != null)
        {
            playerTransform = playerController.transform;
            
            // Ignora colisão com o CharacterController do player
            CharacterController playerCharController = playerController.GetComponent<CharacterController>();
            if (playerCharController != null)
            {
                Physics.IgnoreCollision(characterController, playerCharController, true);
            }
        }
        else
        {
            Debug.LogWarning("Nenhum player encontrado para o inimigo seguir!");
        }
    }

    void Update()
    {
        // Verifica se ainda tem referência ao player
        if (playerController == null || playerTransform == null)
        {
            FindTargetPlayer(); // Tenta encontrar novamente
            if (playerController == null) return; // Se ainda não tem, sai
        }
        
        if (isDead)
        {
            HandleDeadMovement();
            return;
        }

        if (!hasNearbyPlayer) CheckPlayerProximity();
        CheckRandomLaneChange();
        MoveTowardsPlayer();
    }

    private void CheckPlayerProximity()
    {
        distanceToPlayer = GetDistanceToPlayer();
        if (distanceToPlayer <= detectionRange)
        {
            hasNearbyPlayer = true;
            Destroy(gameObject, timeToDestroy);
        }
    }

    // Lógica separada para quando o inimigo está morto (Knockback + Gravidade)
    private void HandleDeadMovement()
    {
        UpdateGravityAndJump();
        Vector3 verticalMovement = Vector3.up * verticalVelocity;

        // Aplica um "drag" (atrito) para ele não deslizar para sempre
        horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * knockbackDrag);

        Vector3 totalMovement = horizontalVelocity + verticalMovement;
        characterController.Move(totalMovement * Time.deltaTime);
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = Vector3.forward;
        Vector3 forwardMovement = direction * speedEnemy;
        
        // Calcula o movimento horizontal normal 
        horizontalVelocity = laneController.CalculateLaneMovement(enemyTransform.position);
        
        UpdateGravityAndJump();
        Vector3 verticalMovement = Vector3.up * verticalVelocity;
        
        Vector3 totalMovement = forwardMovement + horizontalVelocity + verticalMovement;
        characterController.Move(totalMovement * Time.deltaTime);
        
        enemyAnimator.SetFloat("Speed", 1f);
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(enemyTransform.position, playerTransform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        // Se já estiver morto, ignora colisões
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            if(playerController != null && !playerController.getIsShield())
            {
                TriggerEnemyAttack();
            }
            else
            {
                Dead();
            }
        }
        else if (other.CompareTag("Obstacle"))
        {
            Jump();
        }
    }

    private void TriggerEnemyAttack()
    {
        Debug.Log("Enemy collided with Player!");   
        audioSource.PlayOneShot(attackSound);
        enemyAnimator.SetFloat("Speed", 0f);         
        enemyAnimator.SetTrigger("Attack");
        playerController.DamagePlayer();
        enemyCollider.enabled = false;
        
        speedEnemy = 0f;
    }

    private void Dead()
    {
        if (isDead) return; 

        isDead = true; 
        onEnemyDeath?.Invoke();
        Debug.Log("Enemy killed by shield!");
        
        speedEnemy = 0f;
        enemyAnimator.SetBool("Dead", true);
        audioSource.PlayOneShot(deathSound);
        
        if (enemyCollider != null) enemyCollider.enabled = false;
        
        ApplyKnockback();
        
        Destroy(gameObject, 2f);
    }

    private void ApplyKnockback()
    {
        // Calcula a direção do knockback (para trás em relação ao inimigo)
        Vector3 knockbackDirection = -transform.forward; 
        
        verticalVelocity = jumpForce * 0.5f; // Pulo leve
        horizontalVelocity = knockbackDirection * knockbackForce; // Define a velocidade inicial do knockback
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
    
    private void Jump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }
    
    private void CheckRandomLaneChange()
    {
        if (Time.time >= nextLaneChangeTime && !laneController.IsChangingLane)
        {
            PerformRandomLaneChange();
            ScheduleNextLaneChange();
        }
    }
    
    private void PerformRandomLaneChange()
    {
        int currentLane = laneController.CurrentLane;
        int laneChangeDirection;
        
        if (currentLane == -1)
        {
            laneChangeDirection = 1;
        }
        else if (currentLane == 1)
        {
            laneChangeDirection = -1;
        }
        else
        {
            laneChangeDirection = Random.value < 0.5f ? -1 : 1;
        }
        
        laneController.MoveLane(laneChangeDirection);
    }
    
    private void ScheduleNextLaneChange()
    {
        float randomInterval = Random.Range(minLaneChangeInterval, maxLaneChangeInterval);
        nextLaneChangeTime = Time.time + randomInterval;
    }
}