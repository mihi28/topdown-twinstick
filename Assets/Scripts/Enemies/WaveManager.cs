using System;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    //wave manager script, handles current wave, number of enemies spawned, and scales up their stats based on the currrent wave
    //make this a singleton so i can acces it s stats anywhere
    public static WaveManager Instance;

    [Header("Wave Settings")]
    [SerializeField] private int baseNumberOfCredits;
    [SerializeField] private int creditScaler; //scales linearly
    [SerializeField] private float enemyHealthScaler;
    [SerializeField] private float enemyArmorScaler;
    [SerializeField] private float enemyDamageScaler;

    [Header("Wave Progression")]
    [SerializeField] public int currentWave = 1;
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

        //stats scale linearly but as an exponent
        int numberOFEnemies = baseNumberOfCredits + (creditScaler * (currentWave - 1));
        float currenHPMult = 1f + (enemyHealthScaler * (currentWave - 1));
        float currenarmMult = 1f + (enemyArmorScaler * (currentWave - 1));
        float currendmgMult = 1f + (enemyDamageScaler * (currentWave - 1));

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
