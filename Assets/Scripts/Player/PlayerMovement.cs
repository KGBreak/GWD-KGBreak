using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    float gravity = -9.81f;
    [SerializeField]
    float groundCheckDistance = 0.1f;
    [SerializeField]
    float movementSmoothing = 0.1f; // Smoothing factor for gooey movement
    [SerializeField]
    float drag = 0.1f; // Drag effect to simulate stickiness

    [Header("Camera")]
    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    float rotationSpeed = 10f;

    Vector2 moveInput;
    Vector3 velocity;
    Vector3 currentVelocity;
    bool isGrounded;
    CharacterController characterController;
    InputSystem_Actions playerInput;

    private float slowdownTimer = 0f;
    private float slowdownInterval = 0.5f; // Interval in seconds for the halt effect
    private float slowdownDuration = 0.3f; // Duration of the slowdown effect
    private bool isSlowingDown = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnMove;
        playerInput.Player.Disable();
    }

    private void Start()
    {
        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        isGrounded = Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out RaycastHit hitInfo, groundCheckDistance);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Calculate movement relative to the camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 targetMove = (forward * moveInput.y + right * moveInput.x).normalized * moveSpeed;

        // Apply smoothing and drag to the movement
        currentVelocity = Vector3.Lerp(currentVelocity, targetMove, movementSmoothing);
        currentVelocity *= (1 - drag);

        // Apply the halt and continue effect
        slowdownTimer += Time.deltaTime;
        if (slowdownTimer >= slowdownInterval)
        {
            isSlowingDown = true;
            slowdownTimer = 0f;
        }

        if (isSlowingDown)
        {
            currentVelocity *= 0.5f; // Reduce speed by half during slowdown
            slowdownDuration -= Time.deltaTime;
            if (slowdownDuration <= 0f)
            {
                isSlowingDown = false;
                slowdownDuration = 0.1f; // Reset slowdown duration
            }
        }

        if (currentVelocity != Vector3.zero)
        {
            // Smoothly rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        characterController.Move(currentVelocity * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}