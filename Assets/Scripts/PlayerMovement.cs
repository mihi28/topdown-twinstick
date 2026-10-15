using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 5f;

    [Header("Components")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private Rigidbody _rb;


    private Vector3 movement;


    public void Start()
    {
        _rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Player/Move");
    }

    public void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        movement = new Vector3(input.x, 0f, input.y).normalized;
    }

    public void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(movement.x, _rb.linearVelocity.y, movement.z) * speed;
    }
}
