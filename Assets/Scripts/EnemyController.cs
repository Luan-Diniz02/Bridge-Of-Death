using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(LaneController))]
[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speedEnemy = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float timeToDestroy = 5f;
    
    [Header("Configurações de Pulo")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 18f;
    
    [Header("Configurações de Mudança de Faixa")]
    [SerializeField] private float minLaneChangeInterval = 2f;
    [SerializeField] private float maxLaneChangeInterval = 5f;
    
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

    void Awake()
    {
        playerTransform = player.transform;        
        enemyTransform = transform;
        playerController = player.GetComponent<PlayerController>();
        enemyAnimator = enemyTransform.GetComponent<Animator>();            
        enemyCollider = GetComponent<Collider>();
        laneController = GetComponent<LaneController>();
        characterController = GetComponent<CharacterController>();
        
        // Ignora colisão física entre CharacterControllers, mas mantém triggers funcionando
        CharacterController playerCharController = player.GetComponent<CharacterController>();
        if (playerCharController != null)
        {
            Physics.IgnoreCollision(characterController, playerCharController, true);
        }
        
        // Define o primeiro momento de mudança de faixa
        ScheduleNextLaneChange();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void MoveTowardsPlayer()
    {
        Vector3 direction = Vector3.forward;
        Vector3 forwardMovement = direction * speedEnemy;
        
        // Calcula o movimento horizontal para mudança de faixa
        horizontalVelocity = laneController.CalculateLaneMovement(enemyTransform.position);
        
        // Aplica gravidade e pulo
        UpdateGravityAndJump();
        Vector3 verticalMovement = Vector3.up * verticalVelocity;
        
        // Combina todos os movimentos e usa CharacterController
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
            // Pula por cima do obstáculo
            Jump();
        }
    }

    private void TriggerEnemyAttack()
    {
        Debug.Log("Enemy collided with Player!");   
        enemyAnimator.SetFloat("Speed", 0f);         
        enemyAnimator.SetTrigger("Attack");
        playerController.DamagePlayer();
        enemyCollider.enabled = false;
    }

    private void Dead()
    {
        Debug.Log("Enemy killed by shield!");
        enemyAnimator.SetBool("Dead", true);
        enemyCollider.enabled = false;
        
        // Para o movimento do inimigo
        speedEnemy = 0f;
        
        // Destroi após a animação de morte (ajuste o tempo conforme sua animação)
        Destroy(gameObject, 2f);
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
        
        if (currentLane == -1) // Faixa esquerda
        {
            laneChangeDirection = 1; // Vai para o meio
        }
        else if (currentLane == 1) // Faixa direita
        {
            laneChangeDirection = -1; // Vai para o meio
        }
        else // Faixa do meio
        {
            laneChangeDirection = Random.value < 0.5f ? -1 : 1; // Escolhe aleatoriamente
        }
        
        laneController.MoveLane(laneChangeDirection);
    }
    
    private void ScheduleNextLaneChange()
    {
        float randomInterval = Random.Range(minLaneChangeInterval, maxLaneChangeInterval);
        nextLaneChangeTime = Time.time + randomInterval;
    }
}
