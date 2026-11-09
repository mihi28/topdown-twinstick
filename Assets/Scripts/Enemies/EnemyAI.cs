using System;
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
    [SerializeField] private float lineOfSightRadius;
    [SerializeField] private LayerMask obstacleLayerMask;

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
        if (handler == null || handler.enemyData == null) return;

            //just checks which state it's in and gives it instructions on what to do
        switch (currentState)
        {
            case State.Chase:
                SetAgentStopped(false);
                HandleMovement();
                CheckAttackRange();

                timeUntilNextAttack = Mathf.Max(0, timeUntilNextAttack - Time.deltaTime);
                break;

            case State.Windup:
            case State.Cooldown:
                SetAgentStopped(true);
                FaceThePlayer();
                HandleSwarmSeparation();
                break;

            case State.Attack:
                SetAgentStopped(true);
                //can change this later in case i want a tracking attack or something
                break;

            default:
                SetAgentStopped(true);
                break;
            
        }

    }

    private void HandleSwarmSeparation()
    {
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

    private bool HasLineOfSight(float distance)
    {
        // Shoot the Raycast
        // We limit the length to 'distanceToPlayer' so it doesn't accidentally hit a wall BEHIND the player.
        //use a spherecast so the ray is thicker

        //change origin so it starts at enemy eye level
        Vector3 origin = transform.position + Vector3.up * 0.5f; 
    
        Vector3 directionToPlayer = target.transform.position - transform.position;
        float actualDistanceToPlayer = directionToPlayer.magnitude;
        Vector3 dirNormalized = directionToPlayer / actualDistanceToPlayer;
        float castDistance = actualDistanceToPlayer - lineOfSightRadius; // shorten the distance so it doesnt hit a wall if the player is hugging it

        // Check if the enemy is ALREADY clipping into a wall/corner
        if (Physics.CheckSphere(origin, lineOfSightRadius, obstacleLayerMask))
        {
            // Enemy is hugging a wall too closely. The bullet would hit the wall instantly.
            Debug.DrawRay(origin, directionToPlayer, Color.red);
            return false;
        }

        if (Physics.SphereCast(transform.position,lineOfSightRadius, dirNormalized, out RaycastHit hit, castDistance, obstacleLayerMask))
        {
            // A wall is blocking the shot! Draw a RED line in the Scene view.
            Debug.DrawRay(transform.position, directionToPlayer, Color.red);
            return false; 
        }

        // The shot is clear! Draw a GREEN line in the Scene view.
        Debug.DrawRay(transform.position, directionToPlayer, Color.green);
        return true;
    }

    private void CheckAttackRange()
    {   
        //checks if the enemy is withtin attack range and if it can attack and then spits out it starts the sequence
        //also draw a ray from the enemy to the player, if it intersects an obstacle then it resumes chasing
        float distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);

        if(HasLineOfSight(distanceToPlayer)){
            if (distanceToPlayer <= handler.attackRange)
            {
                agent.isStopped = true; // Stop walking

                if (timeUntilNextAttack <= 0f)
                {
                    StartAttackSequence();
                }
            }
            else
            {
                // If out of range, OR behind a wall, keep chasing!
                ResumeChase();
            }
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
        FaceThePlayer();
        HandleSwarmSeparation();
    }

    private void FaceThePlayer()
    {
        // makes the enemy turn towards the player during the windup animation

        if (target == null || handler.enemyData == null) return;

        //draw a ray because i want to see where the enemies look at while moving.
        Debug.DrawRay(transform.position, transform.forward.normalized * handler.attackRange, Color.red);

        Vector3 directionToPlayer = target.transform.position - transform.position;
        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

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

    private void SetAgentStopped(bool stop)
{
    // Only give the command if the agent is fully awake and on the mesh
    if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
    {
        agent.isStopped = stop;
    }
}

}