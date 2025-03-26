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
                player.transform.position = targetPoint.position + DirectionHelper.GetWorldDirection(exitDirection);
                characterController.enabled = true; // Re-enable the CharacterController
            }
            else
            {
                player.transform.position = targetPoint.position + DirectionHelper.GetWorldDirection(exitDirection);
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