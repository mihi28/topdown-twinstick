using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System;
using UnityEngine.AI;

public class EnemyHandler : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float currentMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentArmour;
    [SerializeField] private float currentDamage;
    [field: SerializeField] public float currentSpeed {get; private set;}
    [SerializeField] public bool isElite;

    [Header("AI Behavior Stats")]
    //[field: SerializeField] public EnemyAttackType AttackType { get; private set; }
    //[field: SerializeField] public float attackRange { get; private set; }
    //[field: SerializeField] public float attackCooldown { get; private set; }


    [Header("Components")] 
    //[SerializeField] private EnemyData _basestats;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private NavMeshAgent agent;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
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

        currentMaxHealth = data.maxHealth;
        currentHealth = data.maxHealth;
        currentArmour = data.armour;
        currentDamage = data.damage;
        currentSpeed = data.speed;
        isElite = false;

        //visual part
        meshRenderer.materials = new Material[]
                                {
                                    data.material
                                };
        gameObject.transform.localScale = new Vector3(data.scale,data.scale,data.scale);

        //setup agent speed so it calculates it's path correctly
        agent.speed = currentSpeed;
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

        //visual part
        meshRenderer.materials = new Material[]
                                {
                                    meshRenderer.material,
                                    data.material,
                                };
        gameObject.transform.localScale = new Vector3(data.scale,data.scale,data.scale);


        //here update agent speed as well so it calculates correctly 
        //also update it's radius so it doesnt bump weirdly around things
        agent.speed = currentSpeed;
        agent.radius *= data.scale;
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
    }
}
