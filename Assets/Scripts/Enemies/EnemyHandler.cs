using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System;
using UnityEngine.AI;

public class EnemyHandler : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [field: SerializeField] public EnemyData enemyData {get; private set;}
    [SerializeField] private float currentMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentArmour;
    [field: SerializeField] public float currentDamage {get; private set;}
    [field: SerializeField] public float currentSpeed {get; set;}
    [field: SerializeField] public float currentTurnSpeed {get; set;}
    [SerializeField] public bool isElite;

    [Header("Attack")]
    [SerializeField] private EnemyAI enemyAI;
    [field: SerializeField] public EnemyAttackSO enemyAttackLogic { get; private set; }
    [field: SerializeField] public EnemyAttackType attackType { get; private set; }
    [field: SerializeField] public float attackRange { get; private set; }
    [field: SerializeField] public float attackDuration { get; private set; }
    [field: SerializeField] public float attackWindupTime { get; private set; }
    [field: SerializeField] public float attackCooldown { get; private set; }
    [field: SerializeField] public float timeBetweenAttacks { get; private set; }

    [Header("Death Rattle")]
    [SerializeField] public AbstractDeathRattleSO deathRattle = null;


    [Header("Components")] 
    //[SerializeField] private EnemyData _basestats;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private NavMeshAgent agent;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<EnemyAI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupEnemy(EnemyData data)
    {
        //function that applies scriptable object data onto the enemy
        enemyData = data;

        currentMaxHealth = data.maxHealth;
        currentHealth = data.maxHealth;
        currentArmour = data.armour;
        currentDamage = data.damage;
        currentSpeed = data.speed;
        currentTurnSpeed = data.turnSpeed;
        isElite = false;

        //setup agent speed so it calculates it's path correctly
        agent.speed = currentSpeed;
        agent.radius = data.scale; //reset radius to base level

        //AI Behaviour Setup
        enemyAI.ResumeChase();
        enemyAI.timeUntilNextAttack = 1;
        enemyAttackLogic = data.attackData;
        attackType = data.enemyAttackType;
        attackRange = data.attackRange;     
        attackWindupTime = data.attackWindupTime;
        attackDuration = data.attackDuration;
        attackCooldown = data.attackCooldown;
        timeBetweenAttacks = data.timeBetweenAttacks;

        //visual part
        meshRenderer.materials = new Material[]
                                {
                                    data.material
                                };
        gameObject.transform.localScale = new Vector3(data.scale,data.scale,data.scale);

        //make the shield prefab inactive
        Transform shield = transform.Find("ShieldRoot");
        if (shield != null)
        {
            shield.gameObject.SetActive(false);
        }

        //apply death rattle effect
        deathRattle = data.deathRattleInfo;
    }

    public void ApplyWaveScaling(float hpMult, float armorMult, float dmgMult)
    {
        //gets applied when spawning an enemy
        currentMaxHealth *= hpMult;
        currentArmour *= armorMult;
        currentDamage *= dmgMult;

        currentHealth = currentMaxHealth;
    }

    public void ApplyEliteStats(EliteData data)
    {
        currentMaxHealth *= data.healthMultiplier;
        currentArmour *= data.armourMultiplier;
        currentDamage *= data.damageMultiplier;
        currentSpeed *= data.speedMultiplier;
        isElite = true;

        //here update agent speed as well so it calculates correctly 
        //also update it's radius so it doesnt bump weirdly around things
        agent.speed = currentSpeed;
        agent.radius *= data.scale;

        //modify ai behaviour based on elite stats
        attackRange *= data.attackRangeMultiplier;
        timeBetweenAttacks *= data.timeBetweenAttacksMultiplier;

        //visual part
        meshRenderer.materials = new Material[]
                                {
                                    meshRenderer.material,
                                    data.material,
                                };
        gameObject.transform.localScale = new Vector3(data.scale,data.scale,data.scale);

        //if the enemy already has a death rattle effect, then elite death rattle doesnt apply? it gets overriden? or maybe it might stack?
        //idk for now it just doesnt apply

        if(deathRattle == null)
        {
            deathRattle = data.deathRattleInfo;
        }
    }

    public void takeDamage(DamageInfo info)
    {
        //if the enemy is dead it cant take damage
        if(enemyAI.currentState == EnemyAI.State.Dead)
            return;

        float actualDamage = info.amount;
        if(!info.ignoreArmour)
            actualDamage -= currentArmour;

        currentHealth -= actualDamage;

        updateHealthBar();

        if(currentHealth <= 0f)
        {
            Die();
        }
    }

    public void updateHealthBar()
    {
        if (healthBarFill != null)
        {
            float healthPercentage = currentHealth / currentMaxHealth;
            healthBarFill.fillAmount = healthPercentage;
        }
    }

    public void ResetEnemy()
    {
        currentHealth = currentMaxHealth;
        updateHealthBar();
    }
    public void Die()
    {
        if(enemyAI.currentState == EnemyAI.State.Dead)
            return;

        //lock the enemy into dead instantly so no more memory leaks
        enemyAI.ForceInterruptAttackCoroutine(EnemyAI.State.Dead);

        //execute death rattle
        if(deathRattle != null)
            deathRattle.ExecuteDeathRattle(transform);

        //reset all of everything, make it go back into the pool, update wave info
        WaveManager.Instance.OnEnemyDeath();
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
}
