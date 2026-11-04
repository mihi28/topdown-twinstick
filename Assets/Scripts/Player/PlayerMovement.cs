
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //movement script that is relative to the position of the camera n a diagonal isomethrig style game
    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashTimer;
    [SerializeField] private float dashDuration;

    [Header("Components")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction dashAction;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerStats _stats;
    //[SerializeField] private Camera mainCamera;

    private Vector3 movement;
    private Transform mainCamTransform;
    private bool isDashing = false;
    private float dashDurationTimer;

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Player/Move");
        dashAction = InputSystem.actions.FindAction("Player/Dash");

        speed = _stats.speed;

        mainCamTransform = Camera.main.transform;
    }

    public void Update()
    {
        if (isDashing)
        {
            dashDurationTimer = Mathf.Max(0, dashDurationTimer - Time.deltaTime);
            if(dashDurationTimer <= 0)
                isDashing = false;
            return;
        }

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

        //dashing logic
        if (dashAction.IsPressed() && dashTimer <= 0f)
        {
            //Debug.Log("dash");
            isDashing = true;
            dashTimer = dashCooldown;
            dashDurationTimer = dashDuration;
        }

        dashTimer = Mathf.Max(0, dashTimer - Time.deltaTime);

    }

    public void FixedUpdate()
    {
        if(isDashing)
            _rb.linearVelocity = new Vector3(movement.x * dashSpeed, _rb.linearVelocity.y, movement.z * dashSpeed);

        else
            _rb.linearVelocity = new Vector3(movement.x * speed, _rb.linearVelocity.y, movement.z * speed);

    }
}
