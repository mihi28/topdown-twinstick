using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance; // public ref so it can get accesed anywhere

    [Header("Pool Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolSize;

    private Queue<GameObject> enemyQueue = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.SetActive(false);
            enemyQueue.Enqueue(obj);
        }
    }

    public GameObject GetEnemy()
    {
        if(enemyQueue.Count > 0)
        {
            GameObject obj = enemyQueue.Dequeue();
            obj.SetActive(true);
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            agent.enabled = true;
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            return obj;
        }
    }

    public GameObject GetEnemy(Vector3 pos)
    {
        if(enemyQueue.Count > 0)
        {
            GameObject obj = enemyQueue.Dequeue();
            obj.SetActive(true);
            obj.transform.position = pos;
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            agent.enabled = true;
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            return obj;
        }
    }

    public void ReturnEnemy(GameObject obj)
    {
        obj.SetActive(false);
        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        agent.enabled = false;
        enemyQueue.Enqueue(obj);
    }
}
