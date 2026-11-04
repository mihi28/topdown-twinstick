
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.Pool;

public class BulletLogic : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private string owner;
    [SerializeField] private float velocity;
    [SerializeField] private float lifetime;
    [SerializeField] private float currentLifetime;
    [SerializeField] private float damage;
    [SerializeField] private float critChance;
    [SerializeField] private float critMultiplier;
    
    

    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //currentLifetime = lifetime;
    }

    public void InitializeBullet(string passedOwner, float passedVelocity, float passedLifetime, float passedDamage, float passedCritChance, float passedCritMult)
    {
        owner = passedOwner;
        velocity = passedVelocity;
        currentLifetime = passedLifetime;
        damage = passedDamage;
        critChance = passedCritChance;
        critMultiplier = passedCritMult;

        rb.linearVelocity = (transform.forward * velocity);// + playerVelocity;

        if(owner == "Player")
        {
            gameObject.layer = 8; //playerbullet layer
        }
        else
        {
            gameObject.layer = 10; //enemybullet layer
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentLifetime -= Time.deltaTime;

        if(currentLifetime <= 0f)
        {
            Die();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageableTarget))
        {
            bool critHit = false;

            if(owner == "Player" && other.CompareTag("Enemy")){
                float rollCrit = Random.Range(0f, 100f);
                critHit = rollCrit <= critChance ? true : false;
                if (critHit)
                {
                    damage *= critMultiplier;
                }
            }

            DamageInfo info = new DamageInfo
            {
                amount = damage,
                isCritical = critHit
            };
            
            damageableTarget.takeDamage(info);
        }

        Die();
    }

    public void Die()
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}
