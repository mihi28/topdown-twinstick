
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyData _stats;
    [SerializeField] public GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        //_stats = GetComponent<EnemyStats>();

        //agent.speed = _stats.speed;

        target = GameObject.FindGameObjectWithTag("Player");

        if(target == null)
        {
            return;
        }
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        agent.SetDestination(target.transform.position);
    }
}
