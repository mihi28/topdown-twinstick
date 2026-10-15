using System;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputAction aimAction;
    [SerializeField] private GameObject pivot;
    [SerializeField] private float distanceFromPivot = 0f;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject ground;
    [SerializeField] private LayerMask layerMask;


    public Vector3 currentAimDirection {get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aimAction = InputSystem.actions.FindAction("Player/Aim");
    }

    // Update is called once per frame
    void Update()
    {
        currentAimDirection = getDirection();
        if (currentAimDirection != Vector3.zero)
        {
            transform.position = pivot.transform.position + (currentAimDirection * distanceFromPivot);
            transform.forward = currentAimDirection; 
        }
    }

    public Vector3 getDirection()
    {
        Vector2 mouseScreenPos = aimAction.ReadValue<Vector2>();
        Ray ray = camera.ScreenPointToRay(mouseScreenPos);

        if(Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 targetPoint = hit.point;


                Vector3 direction = (targetPoint - pivot.transform.position).normalized;
                direction.y = 0;
                return direction;
            }
        }
        return Vector3.zero;
    }
}
