
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyStats _stats;
    [SerializeField] public GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<EnemyStats>();

        agent.speed = _stats.speed;

        target = GameObject.FindGameObjectWithTag("Player");

        if(target == null)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        agent.SetDestination(target.transform.position);
    }
}
