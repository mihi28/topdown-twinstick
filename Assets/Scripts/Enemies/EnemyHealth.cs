using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float currentHealth = 0f;

    [Header("Components")] 
    [SerializeField] private EnemyStats _stats;
    [SerializeField] private Image healthBarFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = _stats.maxHealth;

        updateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(DamageInfo info)
    {
        float actualDamage = info.amount - _stats.armour;

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
            float healthPercentage = currentHealth / _stats.maxHealth;
            healthBarFill.fillAmount = healthPercentage;
        }
    }

    public void ResetEnemy()
    {
        currentHealth = _stats.maxHealth;
    }
    private void Die()
    {
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }
}
