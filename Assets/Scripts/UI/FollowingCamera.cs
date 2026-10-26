using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class FollowingCamera : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f,0f,0f);

    public void Update(){
        
        if(target == null)
        {
            return;
        }

        transform.position = target.transform.position + offset;
        transform.LookAt(target);
        
    }
}
