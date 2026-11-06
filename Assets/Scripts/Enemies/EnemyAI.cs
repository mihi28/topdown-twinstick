using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    //this whole script is basically a simple state machine ai. the agic about attacking comes from the scriptable objects mostly
    //this whole codebase is starting to get pretty complicated i m barely keeping up with it
    //basically this script stops the enemy from chasing around the player if it's within attack range. 
    //multiple states so i can actually keep track of whats hapenning with the enemy and be able to set animations later

    [Header("Swarm Settings")]
    [SerializeField] private float separationRadius; // Should match Agent Radius
    [SerializeField] private float separationForce = 5f;

    public enum State {Chase,Windup, Attack, Cooldown, Dead};
    [Header("AI Behaviour Settings")]
    [field: SerializeField] public State currentState {get; private set;}
    [SerializeField] private float timeUntilNextAttack;

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyHandler handler;
    [SerializeField] public GameObject target;
    [SerializeField] private Rigidbody rb;

    private Coroutine attackCoroutine;

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

        currentState = State.Chase;
    }

    void Update()
    {
            //just checks which state it's in and gives it instructions on what to do
        switch (currentState)
        {
            case State.Chase:
                agent.isStopped = false;
                HandleMovement();
                CheckAttackRange();

                timeUntilNextAttack = Mathf.Max(0, timeUntilNextAttack - Time.deltaTime);
                break;

            default:
                agent.isStopped = true;
                break;
            
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

    private void CheckAttackRange()
    {   
        //checks if the enemy is withtin attack range and if it can attack and then spits out it starts the sequence
        if(Vector3.Distance(transform.position, target.transform.position) <= handler.attackRange && timeUntilNextAttack <= 0f)
        {
            StartAttackSequence();
        }
    }

    private void HandleMovement()
    {
        //handles nav mesh movement
        if (target == null) return;

        // Update the path periodically rather than every frame
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateDelay)
        {
            agent.SetDestination(target.transform.position);
            pathUpdateTimer = 0f;
        }
        transform.rotation.SetLookRotation(target.transform.position);

    }

    private void StartAttackSequence()
    {
        //starts a coroutine (dont really understand what those are rn)
        if(currentState == State.Chase)
        {
            attackCoroutine = StartCoroutine(AttackTimeline());
            timeUntilNextAttack = handler.timeBetweenAttacks;
        }
    }

    private IEnumerator AttackTimeline()
    {
        currentState = State.Windup;
        //show the attack telegraph
        //set enemy animation to windup
        yield return new WaitForSeconds(handler.enemyData.attackWindupTime);

        currentState = State.Attack;
        //show attack 
        //set animation to attacking
        handler.enemyAttackLogic.ExecuteAttack(transform, target.transform, handler.enemyData);
        yield return new WaitForSeconds(handler.enemyData.attackDuration);

        currentState = State.Cooldown;
        //set animation to cooldown or like idle
        yield return new WaitForSeconds(handler.enemyData.attackCooldown);

        currentState = State.Chase;
    }

    public void ForceInterruptAttackCoroutine(State newState)
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        currentState = newState;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public void ResumeChase()
    {
        currentState = State.Chase;
    }

}