using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class FollowingCamera : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float distanceFromPlayer;

    public void LateUpdate(){
        
        if(target == null)
        {
            return;
        }

        transform.position = target.position - (transform.forward * distanceFromPlayer);
        
    }
}
