using System.Collections;
using UnityEngine;

public class LookToSides : Actions
{
    [SerializeField] private float rotateSpeed = 90f;    // Degrees per second.
    [SerializeField] private float turnDelay = 1.5f;       // Delay in seconds between rotations.

    public override IEnumerator PerformAction()
    {
        isInterrupted = false;
        // Store the starting (original) rotation.
        Quaternion originalRotation = transform.rotation;
        // Compute target rotations relative to the original.
        Quaternion rightRotation = originalRotation * Quaternion.Euler(0f, 90f, 0f);
        Quaternion leftRotation = originalRotation * Quaternion.Euler(0f, -90f, 0f);

        while (!isInterrupted)
        {
            // Rotate to the right target.
            yield return RotateToTarget(rightRotation);
            yield return new WaitForSeconds(turnDelay);

            // Rotate back to original.
            yield return RotateToTarget(originalRotation);
            yield return new WaitForSeconds(turnDelay);

            // Rotate to the left target.
            yield return RotateToTarget(leftRotation);
            yield return new WaitForSeconds(turnDelay);

            // Rotate back to original.
            yield return RotateToTarget(originalRotation);
            yield return new WaitForSeconds(turnDelay);
        }
    }

    // Helper coroutine to rotate smoothly toward a target rotation.
    private IEnumerator RotateToTarget(Quaternion target)
    {
        // Continue until the angle difference is very small.
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
