using UnityEngine.UI;
using UnityEngine;

public class EnemyHandler : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float currentMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentArmour;
    [SerializeField] private float currentDamage;
    [SerializeField] private float currentSpeed;


    [Header("Components")] 
    //[SerializeField] private EnemyData _basestats;
    [SerializeField] private Image healthBarFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_basestats = GetComponent<EnemyStats>();
        //SetupEnemy(_basestats);

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

        gameObject.GetComponent<EnemyMovement>().SetSpeed(currentSpeed);
    }

    public void ApplyWaveScaling(float hpMult, float armorMult, float dmgMult)
    {
        //gets applied when spawning an enemy
        currentMaxHealth *= hpMult;
        currentArmour *= armorMult;
        currentDamage *= dmgMult;

        currentHealth = currentMaxHealth;
    }

    public void takeDamage(DamageInfo info)
    {
        float actualDamage = info.amount - currentArmour;

        if(actualDamage < 0)
        {
            actualDamage = 0;
        }

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
        //resets ALL stats to base stats
        currentHealth = currentMaxHealth;
        updateHealthBar();
    }
    private void Die()
    {
        WaveManager.Instance.OnEnemyDeath();
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
}
