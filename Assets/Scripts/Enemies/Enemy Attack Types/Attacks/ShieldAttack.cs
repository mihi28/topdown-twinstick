using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[CreateAssetMenu(fileName = "ShieldAttack", menuName = "Scriptable Objects/ShieldAttack")]
public class ShieldAttack : EnemyAttackSO
{
    [Header("Settings")]
    [SerializeField] float shieldMovementSpeed;
    [SerializeField] float shieldRotationSpeed;
    [SerializeField] float shieldUptimeDuration;

    public override void ExecuteAttack(Transform attacker, Transform target, EnemyData data)
    {
        //start a coroutine so the shield is not permanent
        MonoBehaviour runner = attacker.GetComponent<MonoBehaviour>();
        if(runner != null)
        {
            runner.StartCoroutine(ShieldCoroutine(attacker, data));
        }

        //throw new System.NotImplementedException();
    }

    public IEnumerator ShieldCoroutine(Transform attacker, EnemyData data){

        //basically just turn the enemy ito a walking riot shield
        //it s a stance so it goes on as for the duration, or untile the enemy dies
        Transform shield = attacker.Find("ShieldRoot");

        if(shield != null)
        {
            shield.gameObject.SetActive(true);
            //set time untile next attack to huge number so it doesnt spam
            if(attacker.TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
            {
                enemyAI.timeUntilNextAttack = 99999f;
            }

            //set it's speed to a crawl
            if(attacker.TryGetComponent<EnemyHandler>(out EnemyHandler handler))
            {
                handler.currentSpeed = shieldMovementSpeed;
                handler.currentTurnSpeed = shieldRotationSpeed;
            }

            if(attacker.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
            {
                agent.speed = shieldMovementSpeed;
                agent.angularSpeed = shieldRotationSpeed;
            }
        }

        yield return new WaitForSeconds(shieldUptimeDuration);

        //reset it's stats
        if(shield != null)
        {
            shield.gameObject.SetActive(false);

            if(attacker.TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
            {
                enemyAI.timeUntilNextAttack = data.timeBetweenAttacks;
            }

            //set it's speed to a crawl
            if(attacker.TryGetComponent<EnemyHandler>(out EnemyHandler handler))
            {
                handler.currentSpeed = data.speed;
                handler.currentTurnSpeed = data.turnSpeed;
            }

            if(attacker.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
            {
                agent.speed = data.speed;
                agent.angularSpeed = data.turnSpeed;
            }

        }

    }
}
