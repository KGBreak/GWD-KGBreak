using UnityEngine;
using Util;
using FMODUnity;

public class VentPoint : Interactable
{
    [SerializeField] private Transform otherPoint;
    [SerializeField] private bool isEnterPoint;
    [SerializeField] private bool canBringItem = false;
    [SerializeField] private ExitDirection exitDirection;

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
        if (targetPoint == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = targetPoint.position + DirectionHelper.GetWorldDirection(exitDirection) * 0.5f;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = targetPoint.position + DirectionHelper.GetWorldDirection(exitDirection) * 0.5f;
            }
        }
    }

    private void PerformAdditionalActions()
    {
        player.GetComponent<PlayerMovement>().resetGravity();

        if (camController != null)
        {
            if (isEnterPoint)
            {
                AudioSnapshotController.Instance.StartSnapshot();
                RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
                camController.SetFirstPerson();
            }
            else
            {
                AudioSnapshotController.Instance.StopSnapshot();
                RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
                camController.SetThirdPerson();
            }
        }
    }
}
