using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float currentHealth;

    [Header("Components")] 
    [SerializeField] private EnemyStats _basestats;
    [SerializeField] private Image healthBarFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _basestats = GetComponent<EnemyStats>();
        currentHealth = _basestats.maxHealth;

        updateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyWaveScaling(float hpMult, float armorMult, float dmgMult)
    {
        //gets applied when spawning an enemy
        _basestats.maxHealth *= hpMult;
        _basestats.armour *= armorMult;
        _basestats.damage *= dmgMult;

        currentHealth = _basestats.maxHealth;
    }

    public void takeDamage(DamageInfo info)
    {
        float actualDamage = info.amount - _basestats.armour;

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
            float healthPercentage = currentHealth / _basestats.maxHealth;
            healthBarFill.fillAmount = healthPercentage;
        }
    }

    public void ResetEnemy()
    {
        //resets ALL stats to base stats
        currentHealth = _basestats.maxHealth;
        updateHealthBar();
    }
    private void Die()
    {
        WaveManager.Instance.OnEnemyDeath();
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
}
