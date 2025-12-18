using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject [] objectPrefab;
    [SerializeField] private float yPosition;
    [SerializeField] private float[] xPosition, zPosition;
    [SerializeField] private int totalObjects = 10;
    [SerializeField] private bool SpawnRoutineEnabled = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeSpawn();
        if (SpawnRoutineEnabled) StartCoroutine(SpawnRoutine());
    }

    private void InitializeSpawn()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            SpawnObjectAtRandomZ();
        }
    }

    private void SpawnObjectAtRandomZ()
    {
        float spawnZ = Random.Range(zPosition[0], zPosition[1]);
        float spawnX = xPosition[Random.Range(0, xPosition.Length)];
        Vector3 spawnPosition = new(spawnX, yPosition, spawnZ);
        int i = Random.Range(0, objectPrefab.Length);
        Instantiate(objectPrefab[i], spawnPosition, Quaternion.identity);
    }

    private void SpawnObject()
    {
        float spawnZ = zPosition[0];
        float spawnX = xPosition[Random.Range(0, xPosition.Length)];
        Vector3 spawnPosition = new(spawnX, yPosition, spawnZ);
        int i = Random.Range(0, objectPrefab.Length);
        Instantiate(objectPrefab[Random.Range(0, objectPrefab.Length)], spawnPosition, Quaternion.identity);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(2f);
        }
    }

    public void StopSpawning()
    {
        SpawnRoutineEnabled = false;
        StopAllCoroutines();
    }
}