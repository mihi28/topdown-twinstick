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
    [field: SerializeField] public float currentSpeed {get; private set;}
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
        isElite = false;

        //setup agent speed so it calculates it's path correctly
        agent.speed = currentSpeed;
        agent.radius = data.scale; //reset radius to base level

        //AI Behaviour Setup
        enemyAI.ResumeChase();
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

    }

    public void takeDamage(DamageInfo info)
    {
        float actualDamage = info.amount - currentArmour;

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
    private void Die()
    {
        WaveManager.Instance.OnEnemyDeath();
        EnemyPool.Instance.ReturnEnemy(gameObject);
        enemyAI.ForceInterruptAttackCoroutine(EnemyAI.State.Dead);
    }
}
