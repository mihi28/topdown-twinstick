
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    //this script handles enemies spawning, choosing how many, where and the pacing
    //in the future, should also choose the type of enemy and apply elite buffs
    //also should choose in the future if it spawns a boss 
    [Header("Enemy Types")]
    [SerializeField] private EnemyData enemyData;

    [Header("Spawning Settings")]
    [SerializeField] public int maxNumberOfEnemiesForThisWave;
    [SerializeField] public int numberOfSpawnedEnemiesSoFar;
    [SerializeField] private int maxNumberOfEnemiesAlive; //gate it so there arent too many enemies at once
    [SerializeField] public bool isSpawning;

    [Header("Pacing Settings")]
    [SerializeField] private AnimationCurve waveSpawnPacingCurve;
    [SerializeField] private float currentIntensity;
    [SerializeField] private float waveDuration;
    [SerializeField] private float waveTimer;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float minDelayMultiplier;
    [SerializeField] private float maxDelayMultiplier;
    [SerializeField] public float spawnCooldown;



    private float hpMult;
    private float armorMult;
    private float dmgMult;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if(!isSpawning) return;

        //if there are less enemies currently alive than the max number and not all the enemies have been spawned then spawn enemies
        if(WaveManager.Instance.currentNumberOfEnemiesAlive < maxNumberOfEnemiesAlive && numberOfSpawnedEnemiesSoFar < maxNumberOfEnemiesForThisWave)
        {
            if(spawnCooldown <= 0){
                SpawnEnemy();

                //calculate spawn delay based on the curve and waveTimer
                //we take the spawning intensity from the curve graph by mapping the current wave progression
                // then we calculcate what the spawn delay should look like based on an average between a minimum and a maximum delay
                float currentWaveProgress = waveTimer/waveDuration;
                currentIntensity = waveSpawnPacingCurve.Evaluate(currentWaveProgress);

                float averageDelay = waveDuration / (float)maxNumberOfEnemiesForThisWave;
                float minDelay = averageDelay * minDelayMultiplier;
                float maxDelay = averageDelay * maxDelayMultiplier;

                spawnDelay = Mathf.Lerp(maxDelay,minDelay,currentIntensity);

                spawnCooldown = spawnDelay;
            }

            //if the number of enemies spawned this wave reaches max number for this wave, stop spawning
            if(numberOfSpawnedEnemiesSoFar == maxNumberOfEnemiesForThisWave)
            {
                StopSpawning();
            }

            //only count down timers if the max number of enemies alive at a time hasnt been reached
            spawnCooldown = Mathf.Max(0, spawnCooldown - Time.deltaTime);
            waveTimer = Mathf.Min(waveDuration, waveTimer + Time.deltaTime);
        }
        
    }

    public void StartSpawning(int numberOFEnemies, float hpMult, float armorMult, float dmgMult)
    {
        //starts the spawning process, using data given by wavemanager
        maxNumberOfEnemiesForThisWave = numberOFEnemies;
        maxNumberOfEnemiesAlive = (int)(maxNumberOfEnemiesForThisWave * 0.8f);
        this.hpMult = hpMult;
        this.armorMult = armorMult;
        this.dmgMult = dmgMult;

        isSpawning = true;
        waveTimer = 0; 
        spawnCooldown = 1f; //brief delay before starting to sapwn
        numberOfSpawnedEnemiesSoFar = 0;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private void SpawnEnemy()
    {
        //spawns an enemy on a random position on the map
        //modifies their stats according to wave manager multipliers

        GameObject enemy = EnemyPool.Instance.GetEnemy(ChooseSpawnLocation());

        if(enemy.TryGetComponent(out EnemyHandler stats))
        {
            stats.SetupEnemy(enemyData);
            stats.ApplyWaveScaling(hpMult,armorMult,dmgMult);
        }

        spawnCooldown = spawnDelay;
        WaveManager.Instance.currentNumberOfEnemiesAlive++;
        numberOfSpawnedEnemiesSoFar ++;
    }

    private Vector3 ChooseSpawnLocation()
    {
        //chooses a random position on the map
        int i = UnityEngine.Random.Range(0, MapGeneration.SpawnableMapTilesPositions.Count);
        return MapGeneration.SpawnableMapTilesPositions[i];
    }

}
