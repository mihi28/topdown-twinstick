using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyHandler handler;
    [SerializeField] public GameObject target;
    [SerializeField] private Rigidbody rb;

    [Header("Swarm Settings")]
    [SerializeField] private float separationRadius; // Should match Agent Radius
    [SerializeField] private float separationForce = 5f;

    // Timer to prevent calculating paths every single frame
    private float pathUpdateDelay = 0.2f;
    private float pathUpdateTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        handler = GetComponent<EnemyHandler>();
    }

    void Start()
    {
        target = GameObject.FindWithTag("Player"); 
        separationRadius = agent.radius;
    }

    void Update()
    {
        if (target == null) return;

        // Update the path periodically rather than every frame
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateDelay)
        {
            agent.SetDestination(target.transform.position);
            pathUpdateTimer = 0f;
        }

        // Manual Swarm Separation
        // Find all enemies immediately touching this one and push them away, or a player
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, separationRadius, LayerMask.GetMask("Enemy", "Player"));
        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.gameObject != gameObject)
            {
                // Calculate direction away from the overlapping enemy
                Vector3 pushDirection = transform.position - enemy.transform.position;
                pushDirection.y = 0f; // Keep it on the ground
                
                // Slide away smoothly
                transform.Translate(pushDirection.normalized * (separationForce * Time.deltaTime), Space.World);
            }
        }
    }
}