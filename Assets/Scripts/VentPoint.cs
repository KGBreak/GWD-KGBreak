using UnityEngine;

public class VentPoint : Interactable
{
    [SerializeField]
    private Transform otherPoint;
    [SerializeField]
    private bool isEnterPoint;

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
        if (isEnterPoint)
        {
            // Additional actions for enter point
        }
        else
        {
            // Additional actions for exit point
        }
    }
}