using UnityEngine;
using Util;

public class VentPoint : Interactable
{
    [SerializeField]
    private Transform otherPoint;
    [SerializeField]
    private bool isEnterPoint;
    [SerializeField]
    private bool canBringItem = false;
    [SerializeField] private ExitDirection exitDirection;
    private static float originalCameraDistance;
    private static bool originalDistanceSet = false;
    private GameObject player;
    private CameraController camController;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camController = player.GetComponentInChildren<CameraController>();
    }

    public override void InteractWith()
    {
        if (otherPoint != null)
        {
            if (!canBringItem)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<ItemManager>().EjectCurrentItem();
            }
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterController characterController = player.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.enabled = false; // Disable the CharacterController to set the position directly
                player.transform.position = targetPoint.position + DirectionHelper.GetWorldDirection(exitDirection) * 0.5f; 
                characterController.enabled = true; // Re-enable the CharacterController
            }
            else
            {
                player.transform.position = targetPoint.position + DirectionHelper.GetWorldDirection(exitDirection) * 0.5f;
            }
        }
    }

    /*private void PerformAdditionalActions2()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().resetGravity();
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
                    cameraController.SetFirstPerson(); // Set distance to zero when entering the vent

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
    }*/

    private void PerformAdditionalActions()
    {
        player.GetComponent<PlayerMovement>().resetGravity();
        if (camController != null)
        {
            if (isEnterPoint)
            {
                camController.SetFirstPerson();
            }
            else
            {
                camController.SetThirdPerson();
            }
        }
    }

}