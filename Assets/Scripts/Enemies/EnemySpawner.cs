
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    //this script handles enemies spawning, choosing how many, where and the pacing
    //in the future, should also choose the type of enemy and apply elite buffs
    //also should choose in the future if it spawns a boss 
    [Header("Wave Data")]
    [SerializeField] private int currentWave;
    [SerializeField] private WaveData defaultWave;
    [SerializeField] private List<WaveData> specialWaves;
    [SerializeField] private EnemyRoster currentEnemyRoster;
    [SerializeField] private List<EliteData> currentAvailableElites;
    [SerializeField, Range(0f,1f)] private float currentEliteSpawnChance;
    //[SerializeField] private int eliteUpgradeCost;

    [Header("Spawning Settings")]
    [SerializeField] public int maxNumberOfCreditsForThisWave;
    [SerializeField] public int numberOfAvailableCredits;
    [SerializeField] private int maxNumberOfEnemiesAlive; //gate it so there arent too many enemies at once
    [SerializeField] private float eliteSpawnMultiplier; //the number the spawn chance grows by each wave
    [SerializeField] private float maxEliteSpawnChance; //gate it so it's never 100%
    [SerializeField] public bool isSpawning;

    [Header("Pacing Settings")]
    [SerializeField] private AnimationCurve waveSpawnPacingCurve;
    [SerializeField] private float currentIntensity;
    [SerializeField] private float waveDuration;
    [SerializeField] private float waveTimer;
    [SerializeField] private float spawnDelay;
    [SerializeField] public float spawnCooldown;
    [SerializeField] private float minDelayMultiplier;
    [SerializeField] private float maxDelayMultiplier;
    

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
        if(WaveManager.Instance.currentNumberOfEnemiesAlive < maxNumberOfEnemiesAlive && numberOfAvailableCredits > 0)
        {
            if(spawnCooldown <= 0){
                SpawnEnemy();

                //calculate spawn delay based on the curve and waveTimer
                //we take the spawning intensity from the curve graph by mapping the current wave progression
                // then we calculcate what the spawn delay should look like based on an average between a minimum and a maximum delay
                float currentWaveProgress = waveTimer/waveDuration;
                currentIntensity = waveSpawnPacingCurve.Evaluate(currentWaveProgress);

                float averageDelay = waveDuration / (float)maxNumberOfCreditsForThisWave;
                float minDelay = averageDelay * minDelayMultiplier;
                float maxDelay = averageDelay * maxDelayMultiplier;

                spawnDelay = Mathf.Lerp(maxDelay,minDelay,currentIntensity);

                spawnCooldown = spawnDelay;
            }

            //if spent all credits, stop spawning
            if(numberOfAvailableCredits == 0)
            {
                StopSpawning();
            }

            //only count down timers if the max number of enemies alive at a time hasnt been reached
            spawnCooldown = Mathf.Max(0, spawnCooldown - Time.deltaTime); //counting down
            waveTimer = Mathf.Min(waveDuration, waveTimer + Time.deltaTime); //counting up
        }
        
    }

    public void StartSpawning(int numberOFCredits, float hpMult, float armorMult, float dmgMult)
    {
        //starts the spawning process, sets wave data, using data given by wavemanager
        SetupWaveVariables();

        maxNumberOfCreditsForThisWave = numberOFCredits;
        maxNumberOfEnemiesAlive = Mathf.Min(30,(int)(maxNumberOfCreditsForThisWave * 0.8f)); //min of 80% or 30 enemies
        this.hpMult = hpMult;
        this.armorMult = armorMult;
        this.dmgMult = dmgMult;

        isSpawning = true;
        waveTimer = 0; 
        spawnCooldown = 1f; //brief delay before starting to sapwn
        numberOfAvailableCredits = maxNumberOfCreditsForThisWave;

    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private EnemySpawnCost ChooseEnemyToSpawn(int availableCredits)
    {
        //function to choose which enemy to spawn
        //returns an EnemySpawnCost object that has enemy data and it s cost

        //filter out available enemies
        List<EnemySpawnCost> affordableEnemies = currentEnemyRoster.availableEnemies.FindAll(e => e.cost <= availableCredits);

        // If the list is empty, we are too broke to buy anything else. End the wave.
        if (affordableEnemies.Count == 0)
        {
            return null;
        }

        // pick a random base enemy from the affordable list
        int randomIndex = Random.Range(0, affordableEnemies.Count);
        EnemySpawnCost chosenEnemy = affordableEnemies[randomIndex];
        return chosenEnemy;
    }

    private void SpawnEnemy()
    {
        //spawns an enemy on a random position on the map
        //modifies their stats according to wave manager multipliers

        GameObject enemyEntity = EnemyPool.Instance.GetEnemy(ChooseSpawnLocation());

        if(enemyEntity.TryGetComponent(out EnemyHandler handler))
        {
            //first choose which enemy to spawn
            EnemySpawnCost enemyToSpawn = ChooseEnemyToSpawn(numberOfAvailableCredits);

            //if it's different than null then we can spawn an enemy, otherwise no more enemies and the wave is ended

            if(enemyToSpawn != null)
            {
                int totalCost = enemyToSpawn.cost;

                //elite upgrade cost is 50% rounded up, might change that later
                int eliteUpgradeCost = Mathf.CeilToInt(enemyToSpawn.cost * 0.5f);

                //dress up enemy entity with chosen enemy data
                handler.SetupEnemy(enemyToSpawn.enemyData);

                //roll for elite spawn chance, if we can afford the elite upgrade
                if(totalCost + eliteUpgradeCost <= numberOfAvailableCredits)
                {
                    //roll for elite upgrade
                    float randomEliteChanceRoll = Random.Range(0f,1f);
                    if(randomEliteChanceRoll <= currentEliteSpawnChance)
                    {
                        //choose a random elite archetype
                        int randomIndex = Random.Range(0, currentAvailableElites.Count);
                        handler.ApplyEliteStats(currentAvailableElites[randomIndex]);

                        //apply elite ugrade cost
                        totalCost += eliteUpgradeCost;
                    }

                    handler.ApplyWaveScaling(hpMult,armorMult,dmgMult);
                }

                spawnCooldown = spawnDelay;
                WaveManager.Instance.currentNumberOfEnemiesAlive++;
                numberOfAvailableCredits -= totalCost;
                
            }
            
        }

    }

    private Vector3 ChooseSpawnLocation()
    {
        //chooses a random position on the map
        int i = UnityEngine.Random.Range(0, MapGeneration.SpawnableMapTilesPositions.Count);
        return MapGeneration.SpawnableMapTilesPositions[i];
    }

    public void SetupWaveVariables()
    {
        //initialize all wave data
        currentWave = WaveManager.Instance.currentWave;

        WaveData waveToUse = defaultWave;

        foreach (WaveData specialWave in specialWaves)
        {
            if (specialWave.waveNumber == currentWave)
            {
                waveToUse = specialWave;
                break;
            }
        }

        currentEnemyRoster = waveToUse.roster;
        currentAvailableElites = waveToUse.availableElites;

        //elite spawn chance increases exponentially every wave, certain waves have 0 spawn chance
        currentEliteSpawnChance = Mathf.Min(maxEliteSpawnChance, waveToUse.eliteSpawnChance * Mathf.Pow(eliteSpawnMultiplier, currentWave - 1));
    }

}
