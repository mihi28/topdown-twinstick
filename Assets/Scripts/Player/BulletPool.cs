using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance; // public ref so it can get accesed anywhere

    [Header("Pool Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize;

    private Queue<GameObject> bulletQueue = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab, transform);
            obj.SetActive(false);
            bulletQueue.Enqueue(obj);
        }
    }

    public GameObject GetBullet()
    {
        if(bulletQueue.Count > 0)
        {
            GameObject obj = bulletQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(bulletPrefab, transform);
            return obj;
        }
    }

    public void ReturnBullet(GameObject obj)
    {
        obj.SetActive(false);
        bulletQueue.Enqueue(obj);
    }
}
