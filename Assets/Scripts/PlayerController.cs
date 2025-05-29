using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float climbSmooth = 10f;
    [SerializeField] private Transform orientation;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isOnWall;
    private Vector3 wallNormal;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += _ => OnJump();
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= _ => OnJump();
        inputActions.Player.Disable();
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        CheckGround();
        CheckWall();
    }

    private void FixedUpdate()
    {
        if (isOnWall && Mathf.Abs(moveInput.y) > 0.1f)
            Climb();
        else if (isGrounded || !isOnWall)
            Move();
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    private void CheckWall()
    {
        RaycastHit hit;
        isOnWall = Physics.Raycast(orientation.position, orientation.forward, out hit, wallCheckDistance) && !isGrounded;
        if (isOnWall)
        {
            wallNormal = hit.normal;
        }
        rb.useGravity = !isOnWall;
    }

    private void Move()
    {
        Vector3 direction = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        Vector3 newVelocity = direction * moveSpeed;
        rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
    }

    private void Climb()
    {
        Vector3 targetVelocity = new Vector3(0f, moveInput.y * climbSpeed, 0f);
        Vector3 smoothVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * climbSmooth);

        // Mantenerse adherido a la pared
        Vector3 wallStickForce = -wallNormal * 5f;
        rb.linearVelocity = new Vector3(smoothVelocity.x + wallStickForce.x, smoothVelocity.y, smoothVelocity.z + wallStickForce.z);
    }

    private void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else if (isOnWall)
        {
            Vector3 jumpDirection = (Vector3.up - wallNormal * 0.3f).normalized;
            rb.AddForce(jumpDirection * jumpForce * 1.2f, ForceMode.Impulse);
            isOnWall = false;
            rb.useGravity = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (orientation == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(orientation.position, orientation.forward * wallCheckDistance);
    }
}


