using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed;

    [Header("Components")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerStats _stats;


    private Vector3 movement;


    public void Start()
    {
        _rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Player/Move");

        speed = _stats.speed;
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
