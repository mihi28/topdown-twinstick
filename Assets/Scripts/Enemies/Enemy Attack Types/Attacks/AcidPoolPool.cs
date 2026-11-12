using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AcidPoolPool : MonoBehaviour
{
    public static AcidPoolPool Instance; // public ref so it can get accesed anywhere

    [Header("Pool Settings")]
    [SerializeField] private GameObject acidPoolPrefab;
    [SerializeField] private int poolSize;

    private Queue<GameObject> acidPoolQueue = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(acidPoolPrefab, transform);
            obj.SetActive(false);
            acidPoolQueue.Enqueue(obj);
        }
    }

    public GameObject GetAcidPool()
    {
        if(acidPoolQueue.Count > 0)
        {
            GameObject obj = acidPoolQueue.Dequeue();
            obj.GetComponent<Collider>().enabled = true; // reset it's collider when it spawn again
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(acidPoolPrefab, transform);
            return obj;
        }
    }

    public void ReturnAcidPool(GameObject obj)
    {
        obj.SetActive(false);
        acidPoolQueue.Enqueue(obj);
    }
}
