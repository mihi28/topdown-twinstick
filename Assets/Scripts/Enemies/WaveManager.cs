using System;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    //wave manager script, handles current wave, number of enemies spawned, and scales up their stats based on the currrent wave
    //make this a singleton so i can acces it s stats anywhere
    public static WaveManager Instance;

    [Header("Wave Settings")]
    [SerializeField] private int baseNumberOfEnemies;
    [SerializeField] private int enemyNumberScaler;
    [SerializeField] private float enemyHealthMultiplier;
    [SerializeField] private float enemyArmorMultiplier;
    [SerializeField] private float enemyDamageMultiplier;

    [Header("WaveProgression")]
    [SerializeField] private int currentWave = 1;
    [SerializeField] public int currentNumberOfEnemiesAlive;

    [Header("Components")]
    [SerializeField] private EnemySpawner spawner;

    void Awake()
    {
        Instance = this;
        spawner = GetComponent<EnemySpawner>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //wait for 3 seconds then start the first wave
        Invoke(nameof(StartWave), 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartWave()
    {
        //scale up enemy stats and numbers based on wave
        //here we can choose wether its a boss wave or not
        int numberOFEnemies = baseNumberOfEnemies + (enemyNumberScaler * (currentWave - 1));
        float currenHPMult = 1f + (enemyHealthMultiplier * (currentWave - 1));
        float currenarmMult = 1f + (enemyArmorMultiplier * (currentWave - 1));
        float currendmgMult = 1f + (enemyDamageMultiplier * (currentWave - 1));

        spawner.StartSpawning(numberOFEnemies, currenHPMult, currenarmMult, currendmgMult);
    }

    public void OnEnemyDeath()
    {
        currentNumberOfEnemiesAlive --;
        //if the no enemies left alive and spawner has no more enemies left to spawn we move on to the next wave
         if(currentNumberOfEnemiesAlive <= 0 && !spawner.isSpawning)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        //set next wave
        currentWave ++;
        //offer respite time, bring up powerup selection ui

        Invoke(nameof(StartWave), 5f);
    }

}
