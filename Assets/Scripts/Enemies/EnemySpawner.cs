using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private int maxNumberOfEnemies;
    public int currentNumberOfEnemies = 0;
    [SerializeField] private float spawnDelay;
    public float spawnCooldown;
    [SerializeField] private float resetSpawnsDelay;
    public float resetTracker = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemy();
        spawnCooldown -= Time.deltaTime;
        resetTracker += Time.deltaTime;
        NextWave();
    }

    private void SpawnEnemy()
    {
        if(currentNumberOfEnemies < maxNumberOfEnemies && spawnCooldown <= 0)
        {
            GameObject enemy = EnemyPool.Instance.GetEnemy(ChooseSpawnLocation());
            //enemy.transform.position = ChooseSpawnLocation();
            spawnCooldown = spawnDelay;
            currentNumberOfEnemies ++;
        }
    }

    private Vector3 ChooseSpawnLocation()
    {
        int i = UnityEngine.Random.Range(0, MapGeneration.SpawnableMapTilesPositions.Count);
        return MapGeneration.SpawnableMapTilesPositions[i];
    }

    private void NextWave()
    {
        if(currentNumberOfEnemies == maxNumberOfEnemies && resetTracker >= resetSpawnsDelay)
        {
            currentNumberOfEnemies = 0;
            maxNumberOfEnemies ++;
            spawnDelay *= 0.7f;
            resetTracker = 0;
        }
    }
}
