using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float yPosition;
    [SerializeField] private float[] xPosition, zPosition;
    [SerializeField] private int enemyCount = 10;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        float spawnZ = Random.Range(zPosition[0], zPosition[1]);
        float spawnX = xPosition[Random.Range(0, xPosition.Length)];
        Vector3 spawnPosition = new(spawnX, yPosition, spawnZ);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
