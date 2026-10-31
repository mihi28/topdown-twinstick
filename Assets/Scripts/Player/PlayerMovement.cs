using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //movement script that is relative to the position of the camera n a diagonal isomethrig style game
    [Header("Stats")]
    [SerializeField] private float speed;

    [Header("Components")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerStats _stats;
    //[SerializeField] private Camera mainCamera;


    private Vector3 movement;
    private Transform mainCamTransform;

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Player/Move");

        speed = _stats.speed;

        mainCamTransform = Camera.main.transform;
    }

    public void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        //get the camera's directions
        Vector3 camForward = mainCamTransform.forward;
        Vector3 camRight = mainCamTransform.right;

        //flatten them to the floor so you don't fly into the sky
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // multiply your input by the camera's axes
        // input.y (W/S) controls forward/back, input.x (A/D) controls left/right
        movement = (camForward * input.y) + (camRight * input.x);
        movement.Normalize();
    }

    public void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(movement.x * speed, _rb.linearVelocity.y, movement.z * speed);
    }

    public Vector3 getPlayerVelocity()
    {
        return _rb.linearVelocity;
    }
}
