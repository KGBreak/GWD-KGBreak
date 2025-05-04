using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class VentPointEDIT : Interactable
{
    [SerializeField] private Transform otherPoint;
    [SerializeField] private bool isEnterPoint;

    private const string AirDuctLoopEventPath = "event:/Airduct";
    public static EventInstance AirDuctLoopInstance;  // <<== static instance!

    // === SNAPSHOT ===
    private const string VentSnapshotPath = "snapshot:/Airduct_Lowpass";
    public static EventInstance VentSnapshotInstance;  // static so only one active at a time

    public override void InteractWith()
    {
        if (otherPoint == null) return;

        TeleportTo(otherPoint);

        if (isEnterPoint)
        {
            PlayEnterVentFX();
            StartVentSnapshot();
        }
        else    // exit‑point
        {
            StopVentLoop();
            StopVentSnapshot();
        }
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
    private void PlayEnterVentFX()
    {
        RuntimeManager.PlayOneShot("event:/Player/EnterMorph");

        // If the loop is already running we don’t want a duplicate.
        if (!AirDuctLoopInstance.isValid())
            AirDuctLoopInstance = RuntimeManager.CreateInstance(AirDuctLoopEventPath);

        if (AirDuctLoopInstance.isValid())
        {
            AirDuctLoopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            AirDuctLoopInstance.release();
            AirDuctLoopInstance.clearHandle();
        }
        AirDuctLoopInstance = RuntimeManager.CreateInstance(AirDuctLoopEventPath);
        AirDuctLoopInstance.start();
    }

    private void StopVentLoop()
    {
        if (AirDuctLoopInstance.isValid())
        {
            AirDuctLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            AirDuctLoopInstance.release();
            AirDuctLoopInstance.clearHandle();
            RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
        }
    }
    private void StartVentSnapshot()
    {
        if (!VentSnapshotInstance.isValid())
        {
            VentSnapshotInstance = RuntimeManager.CreateInstance(VentSnapshotPath);
        }

        VentSnapshotInstance.start();
    }

    private void StopVentSnapshot()
    {
        if (VentSnapshotInstance.isValid())
        {
            VentSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            VentSnapshotInstance.release();
            VentSnapshotInstance.clearHandle();
        }
    }
}
