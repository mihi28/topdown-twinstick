
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assemblies;

public class BulletLogic : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private float velocity = 0f;
    [SerializeField] private float lifetime = 0f;
    [SerializeField] private float currentLifetime = 0f;
    [SerializeField] private float damage = 0f;
    
    

    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

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
}
