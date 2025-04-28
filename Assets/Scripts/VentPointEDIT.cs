using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class VentPointEDIT : Interactable
{
    [SerializeField] private Transform otherPoint;
    [SerializeField] private bool isEnterPoint;

    private const string AirDuctLoopEventPath = "event:/Airduct";
    public static EventInstance AirDuctLoopInstance;  // <<== static instance!

    public override void InteractWith()
    {
        if (otherPoint == null) return;

        TeleportTo(otherPoint);
        PerformAdditionalActions();
        RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
        AirDuctLoopInstance = RuntimeManager.CreateInstance(AirDuctLoopEventPath);
        AirDuctLoopInstance.start();
    }

    private void TeleportTo(Transform targetPoint)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = false;

        player.transform.position = targetPoint.position;

        if (characterController != null) characterController.enabled = true;
    }

    private void PerformAdditionalActions()
    {
        if (isEnterPoint)
        {

        }
        else
        {
            if (AirDuctLoopInstance.isValid())
            {
                AirDuctLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                AirDuctLoopInstance.release();
            }
        }
    }
}
