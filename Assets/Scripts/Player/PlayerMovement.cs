﻿using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    float moveSpeed = 5f;
    [SerializeField]
    float gravity = -9.81f;
    private float aplliedGravity;

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

    [SerializeField]
    HidingManager hidingManager;

    [SerializeField] private PlayerMovement playerMovement;
    public bool isGameOver = false;


    Vector2 moveInput ;
    Vector3 velocity;
    Vector3 currentVelocity;
    bool isGrounded;
    CharacterController characterController;

    public Vector2 MoveInput => moveInput;

    private float slowdownTimer = 0f;
    private float slowdownInterval = 0.5f; // Interval in seconds for the halt effect
    private float slowdownDuration = 0.3f; // Duration of the slowdown effect
    private bool isSlowingDown = false;
    private bool isHiding = false;


    // Added for FMOD
    private EventInstance movementEvent;
    private bool isMovementEventPlaying = false;

    [Header("Respawn")]
    private Vector3 lastCheckpointPosition;
    public Animator gameOverAnimator;
    private EnemyVision enemyToReset;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        lastCheckpointPosition = transform.position; // Initialize the last checkpoint position to the player's starting position
    }

    private void Start()
    {
        // Hide and lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        aplliedGravity = gravity;
        // Create the movement event
        movementEvent = RuntimeManager.CreateInstance("event:/Player/Movement");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isHiding) {
            if (context.phase == InputActionPhase.Performed)
            {
                moveInput = Vector2.zero;
                hidingManager.MoveHidingObject();
            }
        }
        else
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    private void Update()
    {
        //if you press R then reset to last checkpoint
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToLastCheckpoint();
        }

        if (isHiding) {
            return;
        }
        // ── STOP ON PAUSE ──
        if (PauseMenu.IsPaused && isMovementEventPlaying)
        {
        movementEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        isMovementEventPlaying = false;
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance);

        if (isGrounded && aplliedGravity < 0f)
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
        velocity.y += aplliedGravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Start/stop the FMOD event
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            if (!isMovementEventPlaying)
            {
                movementEvent.start();
                isMovementEventPlaying = true;
            }
        }
        else
        {
            if (isMovementEventPlaying)
            {
                movementEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                RuntimeManager.PlayOneShot("event:/Player/Movement_Stop", transform.position);
                isMovementEventPlaying = false;
            }
        }
    }

    public void SetHiding(bool isHidingInput)
    {
        if (isHidingInput && isMovementEventPlaying)
        {
            movementEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            isMovementEventPlaying = false;
            RuntimeManager.PlayOneShot("event:/Player/Movement_Stop", transform.position);
        }

        isHiding = isHidingInput;
    }


    public bool getHiding()
    {
        return isHiding;
    }

    public void SetGravity(float newGravity)
    {
        this.aplliedGravity = newGravity;
    }

    public void resetGravity()
    {
        velocity.y = 0f;
        this.aplliedGravity = gravity;
    }
    public void ForceStopMovementAudio()
    {
        if (isMovementEventPlaying)
        {
            movementEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            isMovementEventPlaying = false;
            RuntimeManager.PlayOneShot("event:/Player/Movement_Stop", transform.position);
        }

        currentVelocity = Vector3.zero;
        velocity = Vector3.zero;
    }

    public void TriggerGameOverSequence(EnemyVision enemyToReset)
    {
        if (isGameOver) return; // Prevent multiple triggers
        isGameOver = true;
        this.enemyToReset = enemyToReset;
        gameOverAnimator.SetBool("isGameOver", isGameOver);
        Invoke(nameof(ResetToLastCheckpoint), 2.5f); // Delay the reset to allow the animation to play
    }

    public void ResetToLastCheckpoint()
    {
        characterController.enabled = false; // Disable the character controller to prevent movement during teleportation
        transform.position = lastCheckpointPosition; // Teleport the player to the last checkpoint
        characterController.enabled = true; // Re-enable the character controller
        cameraTransform.GetComponent<CameraController>().SetFirstPerson();
        isGameOver = false; // Reset the game over state
        enemyToReset.detectionMeterValue = 0f; // Reset the enemy's detection meter
        gameOverAnimator.SetBool("isGameOver", isGameOver);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpointPosition = other.transform.GetChild(0).position;
        }
    }
}