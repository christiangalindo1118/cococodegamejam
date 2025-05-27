using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float climbForce = 8f;
    public float wallDetectionDistance = 0.5f;

    [Header("Component References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform orientation;
    
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isOnWall;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += ctx => HandleJump();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        CheckWall();
    }

    private void FixedUpdate()
    {
        if (!isOnWall)
        {
            HandleMovement();
        }
    }

    private void CheckWall()
    {
        RaycastHit hit;
        isOnWall = Physics.Raycast(orientation.position, orientation.forward, 
                    out hit, wallDetectionDistance) && hit.collider.CompareTag("Wall");
        
        // Pegar a la pared
        if(isOnWall && !isGrounded)
        {
            rb.useGravity = false;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
    }

    private void HandleJump()
    {
        if(isOnWall)
        {
            // Escalar en Y
            rb.AddForce(Vector3.up * climbForce, ForceMode.Impulse);
        }
        else if(isGrounded)
        {
            // Salto normal
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(orientation.position, orientation.forward * wallDetectionDistance);
    }
}