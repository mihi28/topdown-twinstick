
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DashAttackSO", menuName = "Scriptable Objects/DashAttackSO")]
public class DashAttackSO : EnemyAttackSO
{
    [Header("Attack Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashDistance;
    [SerializeField] private float hitboxSize;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask targetLayer;

    public override void ExecuteAttack(Transform attacker, Transform target, EnemyData data)
    {
        
        MonoBehaviour runner = attacker.GetComponent<MonoBehaviour>();
        if(runner != null)
        {
            runner.StartCoroutine(DashCoroutine(attacker, data));
        }
        //throw new System.NotImplementedException();
    }

    private IEnumerator DashCoroutine(Transform attacker, EnemyData data)
    {
        NavMeshAgent agent = attacker.GetComponent<NavMeshAgent>();
        Rigidbody rb = attacker.GetComponent<Rigidbody>();
        Collider col = attacker.GetComponent<Collider>();

        // Prepare for the dash
        if (agent != null) agent.enabled = false; 
        
        // Make them a ghost so they easily slide through the player and other enemies
        if (col != null) col.isTrigger = true; 

        Vector3 dashDirection = attacker.forward.normalized;
        float actualDashDistance = dashDistance;

        // WALL CHECK: Fix the math so it stops AT the wall
        if(Physics.Raycast(attacker.position, dashDirection, out RaycastHit hit, dashDistance, obstacleMask))
        {
            actualDashDistance = hit.distance;
        }

        // CACHE THE START POSITION FOR THE LERP!
        Vector3 startPos = attacker.position;
        Vector3 targetPos = startPos + (dashDirection * actualDashDistance);
        
        // Hit memory so we don't one-shot the player by hitting them 50 times
        HashSet<Collider> alreadyHit = new HashSet<Collider>();

        float timer = 0f;
        while (timer < dashDuration)
        {
            // Use fixedDeltaTime since we are waiting for FixedUpdate!
            timer += Time.fixedDeltaTime; 
            float percent = timer / dashDuration;

            // Move smoothly from the CONSTANT startPos
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, percent));

            // Detect Hits
            Collider[] hits = Physics.OverlapSphere(attacker.position, hitboxSize, targetLayer);
            foreach(Collider hitCol in hits)
            {
                // Only register the hit if we haven't hit this specific object yet
                if (!alreadyHit.Contains(hitCol))
                {
                    alreadyHit.Add(hitCol);
                    Debug.Log("Player hit by dash!");

                    if(hitCol.TryGetComponent(out PlayerHandler playerHandler)){
                        DamageInfo damageInfo = new DamageInfo
                        {
                            amount = data.damage
                        };
                        playerHandler.takeDamage(damageInfo);
                    }
                }
            }

            yield return new WaitForFixedUpdate(); 
        }

        // Snap perfectly to the end
        rb.MovePosition(targetPos);
        
        // Return everything to normal
        if (col != null) col.isTrigger = false;
        if (agent != null) agent.enabled = true;
    }
}