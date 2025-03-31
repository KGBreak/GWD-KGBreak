using UnityEngine;
using System.Collections;

public class EyePhysics : MonoBehaviour
{
    // Parameters for idle rotation behavior
    public float idleRotationStrength = 1.0f; // Amplitude of the idle rotation
    public float idleRotationSpeed = 0.5f;    // Speed (frequency) of the idle rotation
    public float minIdleWaitTime = 0.5f;      // Minimum wait time between idle rotations
    public float maxIdleWaitTime = 2.0f;      // Maximum wait time between idle rotations
    public float maxIdleRotationX = 10.0f;    // Maximum idle rotation angle on the x-axis
    public float maxIdleRotationY = 10.0f;    // Maximum idle rotation angle on the y-axis

    private Quaternion initialRotation;        // Initial rotation of the eye

    // Random seeds to produce unique idle patterns per eye instance
    private float randomSeedX;
    private float randomSeedY;

    void Start()
    {
        initialRotation = transform.localRotation;

        // Initialize random seeds for unique idle movement per eye
        randomSeedX = Random.Range(0f, 100f);
        randomSeedY = Random.Range(0f, 100f);

        // Start the idle rotation coroutine
        StartCoroutine(IdleRotationCoroutine());
    }

    private IEnumerator IdleRotationCoroutine()
    {
        while (true)
        {
            if (PauseMenu.IsPaused)
            {
                yield return null;
                continue;
            }

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