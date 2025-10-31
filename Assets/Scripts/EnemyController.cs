using UnityEditor.Animations;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speedEnemy = 3f;
    private Transform playerTransform;
    private Transform enemyTransform;
    private Animator enemyAnimator;    
    private float distanceToPlayer;
    private PlayerController playerController;
    private Collider enemyCollider;

    void Awake()
    {
        playerTransform = player.transform;
        playerController = player.GetComponent<PlayerController>();
        enemyTransform = transform;
        enemyAnimator = enemyTransform.GetComponent<Animator>();            
        enemyCollider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        distanceToPlayer = GetDistanceToPlayer();
        if (distanceToPlayer > 2f)
        {
            Vector3 direction = (playerTransform.position - enemyTransform.position).normalized;
            direction.x = 0;
            direction.y = 0;
            enemyTransform.position += direction * speedEnemy * Time.deltaTime;
            enemyAnimator.SetFloat("Speed", 1f);
        }
        else
        {
            enemyAnimator.SetFloat("Speed", 0f);
        }
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
