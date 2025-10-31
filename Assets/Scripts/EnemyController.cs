using UnityEditor.Animations;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Transform playerTransform;
    private Transform enemyTransform;
    private Animator enemyAnimator;    
    private float distanceToPlayer;


    void Awake()
    {
        playerTransform = player.transform;
        enemyTransform = transform;
        enemyAnimator = enemyTransform.GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = GetDistanceToPlayer();
        Debug.Log("Distance to Player: " + distanceToPlayer);

        if (distanceToPlayer < 2.0f)
        {
            enemyAnimator.SetTrigger("Attack");
        }
    }
    
    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(enemyTransform.position, playerTransform.position);
    }
}
