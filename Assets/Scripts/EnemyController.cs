using UnityEditor.Animations;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject player;
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
        /* distanceToPlayer = GetDistanceToPlayer();
        //Debug.Log("Distance to Player: " + distanceToPlayer);

        attackTimer += Time.deltaTime;
        if (distanceToPlayer < 2.0f && attackTimer >= delayAttack)
        {
            enemyAnimator.SetTrigger("Attack");
            attackTimer = 0.0f;
        } */
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(enemyTransform.position, playerTransform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy collided with Player!");
            enemyAnimator.SetTrigger("Attack");
            playerController.DamagePlayer();
            enemyCollider.enabled = false;
        }
    }
}
