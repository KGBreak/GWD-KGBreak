using UnityEngine;
using System.Collections;

public class EyePhysics : MonoBehaviour
{
    public Transform playerTransform;    // Reference to the player's transform
    public float dragAmount = 0.1f;        // Multiplier to convert player speed into a drag offset
    public float maxHorizontalDragDistance = 0.5f;   // Maximum allowed horizontal drag offset
    public float maxVerticalDragDistance = 0.3f;     // Maximum allowed vertical drag offset
    public float targetLerpSpeed = 5.0f;   // Base speed at which the target offset is updated
    public float springStrength = 30.0f;   // Spring strength for smoothing
    public float damping = 5.0f;           // Damping factor for the spring system
    public float rotationDragFactor = 1.0f; // Additional factor for rotation drag effect

    // New parameters for idle floating behavior
    public float idleFloatStrength = 0.05f;  // Amplitude of the idle floating offset
    public float idleFloatSpeed = 1.0f;      // Speed (frequency) of the idle floating oscillation
    public float idleRotationStrength = 1.0f; // Amplitude of the idle rotation
    public float idleRotationSpeed = 0.5f;    // Speed (frequency) of the idle rotation
    public float minIdleWaitTime = 0.5f;      // Minimum wait time between idle rotations
    public float maxIdleWaitTime = 2.0f;      // Maximum wait time between idle rotations
    public float maxIdleRotationX = 10.0f;    // Maximum idle rotation angle on the x-axis
    public float maxIdleRotationY = 10.0f;    // Maximum idle rotation angle on the y-axis

    private Vector3 originalLocalPosition;   // Center position of the eye relative to the player
    private Vector3 lastPlayerPosition;        // Player position in the previous frame
    private Quaternion lastPlayerRotation;     // Player rotation in the previous frame
    private Vector3 velocity;                  // Current velocity of the eye movement
    private Vector3 targetOffset;              // Current accumulated offset from the center
    private Quaternion initialRotation;        // Initial rotation of the eye

    // Random seeds to produce unique idle patterns per eye instance
    private float randomSeedX;
    private float randomSeedY;
    private float randomSeedZ;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned.");
            enabled = false;
            return;
        }

        originalLocalPosition = transform.localPosition;
        lastPlayerPosition = playerTransform.position;
        lastPlayerRotation = playerTransform.rotation;
        velocity = Vector3.zero;
        targetOffset = Vector3.zero;
        initialRotation = transform.localRotation;

        // Initialize random seeds for unique idle movement per eye
        randomSeedX = Random.Range(0f, 100f);
        randomSeedY = Random.Range(0f, 100f);
        randomSeedZ = Random.Range(0f, 100f);

        // Start the idle rotation coroutine
        StartCoroutine(IdleRotationCoroutine());
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        // Calculate the player's movement and speed
        Vector3 playerMovement = playerTransform.position - lastPlayerPosition;
        float playerSpeed = playerMovement.magnitude / Time.deltaTime;
        float dragDistance = Mathf.Clamp(playerSpeed * dragAmount, 0, Mathf.Max(maxHorizontalDragDistance, maxVerticalDragDistance));

        // Calculate the desired offset from the player's movement
        Vector3 desiredOffset = Vector3.zero;
        if (playerSpeed > 0.001f)
        {
            Vector3 playerMovementDirection = playerMovement.normalized;
            Vector3 localDragDirection = playerTransform.InverseTransformDirection(playerMovementDirection);
            desiredOffset = -localDragDirection * dragDistance;
        }

        // Calculate rotational drag from changes in player rotation
        Quaternion rotationDifference = playerTransform.rotation * Quaternion.Inverse(lastPlayerRotation);
        Vector3 rotationDrag = rotationDifference * Vector3.forward * dragDistance * rotationDragFactor;
        desiredOffset += rotationDrag;

        // Calculate the distance to the target offset
        float distanceToTarget = Vector3.Distance(targetOffset, desiredOffset);

        // Adjust the lerp speed based on the distance to the target using a cubic easing function
        float t = Mathf.Clamp01(distanceToTarget / Mathf.Max(maxHorizontalDragDistance, maxVerticalDragDistance));
        float easedT = t * t * (3f - 2f * t); // Cubic easing function

        // Smoothly update the target offset with dynamic lerp speed
        targetOffset = Vector3.Lerp(targetOffset, desiredOffset, easedT * targetLerpSpeed * Time.deltaTime);

        // Create an idle floating offset using Perlin noise
        float idleOffsetX = (Mathf.PerlinNoise(Time.time * idleFloatSpeed + randomSeedX, 0f) - 0.5f) * 2f * idleFloatStrength;
        float idleOffsetY = (Mathf.PerlinNoise(Time.time * idleFloatSpeed + randomSeedY, 0f) - 0.5f) * 2f * idleFloatStrength;
        float idleOffsetZ = (Mathf.PerlinNoise(Time.time * idleFloatSpeed + randomSeedZ, 0f) - 0.5f) * 2f * idleFloatStrength;
        Vector3 idleOffset = new Vector3(idleOffsetX, idleOffsetY, idleOffsetZ);

        // The final target position is the original position plus both movement and idle offsets
        Vector3 targetLocalPosition = originalLocalPosition + targetOffset + idleOffset;

        // Clamp the target local position based on the maximum allowed drag distances
        targetLocalPosition.x = Mathf.Clamp(targetLocalPosition.x, originalLocalPosition.x - maxHorizontalDragDistance, originalLocalPosition.x + maxHorizontalDragDistance);
        targetLocalPosition.y = Mathf.Clamp(targetLocalPosition.y, originalLocalPosition.y - maxVerticalDragDistance, originalLocalPosition.y + maxVerticalDragDistance);

        // Apply the spring-damping system to smoothly move the eye toward the target position
        Vector3 displacement = targetLocalPosition - transform.localPosition;
        Vector3 springForce = displacement * springStrength;
        Vector3 dampingForce = velocity * damping;
        Vector3 force = springForce - dampingForce;

        velocity += force * Time.deltaTime;
        transform.localPosition += velocity * Time.deltaTime;

        // Update the last frame's player position and rotation for the next frame
        lastPlayerPosition = playerTransform.position;
        lastPlayerRotation = playerTransform.rotation;
    }

    private IEnumerator IdleRotationCoroutine()
    {
        while (true)
        {
            // Apply idle rotation using Perlin noise
            float idleRotationX = (Mathf.PerlinNoise(Time.time * idleRotationSpeed + randomSeedX, 0f) - 0.5f) * 2f * idleRotationStrength;
            float idleRotationY = (Mathf.PerlinNoise(Time.time * idleRotationSpeed + randomSeedY, 0f) - 0.5f) * 2f * idleRotationStrength;

            // Clamp the idle rotation values to the specified limits
            idleRotationX = Mathf.Clamp(idleRotationX, -maxIdleRotationX, maxIdleRotationX);
            idleRotationY = Mathf.Clamp(idleRotationY, -maxIdleRotationY, maxIdleRotationY);

            Quaternion idleRotation = Quaternion.Euler(idleRotationX, idleRotationY, 0f);
            transform.localRotation = initialRotation * idleRotation;

            // Wait for a random duration before the next rotation
            float waitTime = Random.Range(minIdleWaitTime, maxIdleWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
}