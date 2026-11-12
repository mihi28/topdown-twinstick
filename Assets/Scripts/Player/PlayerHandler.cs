using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System;
using UnityEngine.AI;

public class PlayerHandler : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [field: SerializeField] public PlayerStats stats {get; private set;}
    [SerializeField] private float currentMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool isDead = false;
    [SerializeField] private float currentArmour;
    [field: SerializeField] public float currentDamage {get; private set;}
    [field: SerializeField] public float currentSpeed {get; set;}


    [Header("Components")] 
    //[SerializeField] private EnemyData _basestats;
    [SerializeField] private Image healthBarFill;
    //[SerializeField] private MeshRenderer meshRenderer;

    void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePlayer(stats);
        updateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayer(PlayerStats stats)
    {
        //initialize the handler with the players stats
        currentMaxHealth = stats.maxHealth;
        currentHealth = currentMaxHealth;
        currentArmour = stats.armour;
        isDead = false;
    }

    public void ApplyUpgrade()
    {
        //function that will get called on the upgrade ui to apply a certain upgrade to the player
    }

    public void takeDamage(DamageInfo info)
    {
        //if the enemy is dead it cant take damage
        if(isDead)
            return;

        float actualDamage = info.amount;
        if(!info.ignoreArmour)
            actualDamage -= currentArmour;

        currentHealth -= actualDamage;

        updateHealthBar();

        //here i can add revenge effects which are also probably gonna be scriptable objects

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

    public void Die()
    {
        if(isDead)
            return;

        //lock the player into dead instantly so no more memory leaks
        isDead = true;

        gameObject.SetActive(false); //just to show that you're dead

        //play death animation and sound
        //show death ui
        //display stats and things and stuff
    }
}
