using Unity.VisualScripting;
using UnityEngine;

public class AcidPoolLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Settings")]
    [SerializeField] private float currentRadius;
    [SerializeField] private float currentDuration;
    [SerializeField] private float durationTimer;
    [SerializeField] private float currentPlayerDamage;
    [SerializeField] private float currentEnemyDamage;
    [SerializeField] private float currentTickFrquency;
    [SerializeField] private float tickTimer;
    [SerializeField] private LayerMask targetLayerMask;

    //global dictionary recording all hit targets 
    //this exists so stacked puddles dont deal double damage
    //gets cleared every DealDamage() call if the time recorded exceeds tick frequency
    private static System.Collections.Generic.Dictionary<GameObject, float> globalAcidHitTimers = new System.Collections.Generic.Dictionary<GameObject, float>();
    private static System.Collections.Generic.List<GameObject> keysToRemove = new System.Collections.Generic.List<GameObject>();

    //reset hits cache on runtime so that no more acid immunity bugs
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        globalAcidHitTimers?.Clear();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(durationTimer == 0)
        {
            Die();
        }

        if(tickTimer == 0)
        {
            DealDamage();
            tickTimer = currentTickFrquency;
        }

        durationTimer = Mathf.Max(0, durationTimer - Time.deltaTime);
        tickTimer = Mathf.Max(0, tickTimer - Time.deltaTime);
    }

    public void InitializeAcidPool(float passedRadius, float passedDuration, float passedTickFrquency, float passedPlayerDamage, float passedEnemyDamage, LayerMask passedTargetLayerMask)
    {
        currentRadius = passedRadius;
        currentDuration = passedDuration;
        currentTickFrquency = passedTickFrquency;
        currentPlayerDamage = passedPlayerDamage;
        currentEnemyDamage = passedEnemyDamage;
        targetLayerMask = passedTargetLayerMask;

        durationTimer = currentDuration;

    }

    public void DealDamage()
    {
        //clear hits cache
        keysToRemove.Clear();
        foreach(var entry in globalAcidHitTimers)
        {
            if(Time.time - entry.Value >= currentTickFrquency)
            {
                keysToRemove.Add(entry.Key);
            }
        }

        foreach(var key in keysToRemove)
        {
            globalAcidHitTimers.Remove(key);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius / 2, targetLayerMask);// /2 so the damage zone matches the prefab mesh size
        //Debug.Log($"<color=orange>[ACID] Ticked! Found {hits.Length} colliders in radius.</color>");

        foreach(Collider hit in hits)
        {
            //comb through hits, damage targets based on their tag
            //enemies should take less than the player

            //check if the entity has been hit already
            GameObject hitTarget = hit.gameObject;

            if(!globalAcidHitTimers.ContainsKey(hitTarget))
                if(hit.TryGetComponent(out IDamageable damageableTarget))
                {

                    //add objectid to hits cache along with time of hit
                    globalAcidHitTimers.Add(hitTarget, Time.time);

                    DamageInfo damageInfo = new DamageInfo{};

                    //make it ignore armour since it's acid
                    damageInfo.ignoreArmour = true;

                    if (hit.CompareTag("Enemy"))
                    {
                        damageInfo.amount = currentEnemyDamage;
                    }
                    
                    else if (hit.CompareTag("Player"))
                    {
                        damageInfo.amount = currentPlayerDamage;
                    }
                    
                    damageableTarget.takeDamage(damageInfo);
                }
        }
    }

    public void Die()
    {
        AcidPoolPool.Instance.ReturnAcidPool(gameObject);
    }
}
