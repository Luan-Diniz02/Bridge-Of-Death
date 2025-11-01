using UnityEditor.Animations;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speedEnemy = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float timeToDestroy = 5f;
    private Transform playerTransform;
    private Transform enemyTransform;
    private Animator enemyAnimator;    
    private float distanceToPlayer;
    private PlayerController playerController;
    private Collider enemyCollider;
    private bool hasNearbyPlayer = false;

    void Awake()
    {
        playerTransform = player.transform;        
        enemyTransform = transform;
        playerController = player.GetComponent<PlayerController>();
        enemyAnimator = enemyTransform.GetComponent<Animator>();            
        enemyCollider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasNearbyPlayer) CheckPlayerProximity();
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
        enemyTransform.position += direction * speedEnemy * Time.deltaTime;
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
            TriggerEnemyAttack();
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
}
