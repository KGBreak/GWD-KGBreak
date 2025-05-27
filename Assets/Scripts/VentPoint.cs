using UnityEngine;
using Util;
using FMODUnity;
using FMOD.Studio;

public class VentPoint : Interactable
{
    [SerializeField]
    private Transform otherPoint;
    [SerializeField]
    private bool isEnterPoint;
    [SerializeField]
    private bool canBringItem = false;
    [SerializeField] private ExitDirection exitDirection;
    [SerializeField] private string snapshotPath = "snapshot:/Airduct_Lowpass";
    private static EventInstance airductLowpassSnapshot;
    private static bool snapshotActive = false;
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

    private void PerformAdditionalActions()
    {
        player.GetComponent<PlayerMovement>().resetGravity();
        if (camController != null)
        {
            if (isEnterPoint)
            {
                airductLowpassSnapshot = RuntimeManager.CreateInstance(snapshotPath);
                airductLowpassSnapshot.start();
                snapshotActive = true;
                RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
                // Tell the AmbientController we�re now in the vent
                AmbientController.Instance.SetInVent(true);
                camController.SetFirstPerson();
            }
            else
            {
                airductLowpassSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                airductLowpassSnapshot.release();
                snapshotActive = false;
                // Tell the AmbientController we�ve exited
                AmbientController.Instance.SetInVent(false);
                // Play the �exit vent� stinger
                RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
                camController.SetThirdPerson();
            }
        }
    }
}
