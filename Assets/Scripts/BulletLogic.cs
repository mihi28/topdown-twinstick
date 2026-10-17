
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assemblies;

public class BulletLogic : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private float velocity;
    [SerializeField] private float lifetime;
    [SerializeField] private float currentLifetime;
    [SerializeField] private float damage;
    [SerializeField] private float critChance;
    [SerializeField] private float critMultiplier;
    
    

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerStats _stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        velocity = _stats.bulletVelocity;
        lifetime = _stats.range;
        damage = _stats.damage;
        critChance = _stats.critChance;
        critMultiplier = _stats.critMultiplier;

        rb.linearVelocity = transform.forward * velocity;
        currentLifetime = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        currentLifetime -= Time.deltaTime;

        if(currentLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageableTarget))
        {
            float rollCrit = Random.Range(0f, 100f);
            bool critHit = rollCrit <= critChance ? true : false;
            if (critHit)
            {
                damage *= critMultiplier;
            }

            DamageInfo info = new DamageInfo
            {
                amount = damage,
                isCritical = critHit
            };
            
            damageableTarget.takeDamage(info);
        }

        Destroy(gameObject);
    }
}
