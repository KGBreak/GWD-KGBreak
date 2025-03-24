using UnityEngine;

public class VentPoint : Interactable
{
    [SerializeField]
    private Transform otherPoint;
    [SerializeField]
    private bool isEnterPoint;

    private static float originalCameraDistance;
    private static bool originalDistanceSet = false;

    public override void InteractWith()
    {
        if (otherPoint != null)
        {
            TeleportTo(otherPoint);
            PerformAdditionalActions();
        }
    }

    private void TeleportTo(Transform targetPoint)
    {
        if (targetPoint == null)
        {
            return;
        }

        Vector3 offset = targetPoint.forward * (GetComponent<Renderer>().bounds.size.z + 0.5f);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterController characterController = player.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.enabled = false; // Disable the CharacterController to set the position directly
                player.transform.position = targetPoint.position + offset;
                characterController.enabled = true; // Re-enable the CharacterController
            }
            else
            {
                player.transform.position = targetPoint.position + offset;
            }
        }
    }

    private void PerformAdditionalActions()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CameraController cameraController = player.GetComponentInChildren<CameraController>();
            if (cameraController != null)
            {
                if (isEnterPoint)
                {
                    if (!originalDistanceSet)
                    {
                        originalCameraDistance = cameraController.distanceFromPlayer;
                        originalDistanceSet = true;
                    }
                    cameraController.distanceFromPlayer = 0.01f; // Set distance to zero when entering the vent
                }
                else
                {
                    if (originalDistanceSet)
                    {
                        cameraController.distanceFromPlayer = originalCameraDistance; // Restore the original distance when exiting the vent
                        originalDistanceSet = false;
                    }
                }
            }
        }
    }
}