using FMODUnity;
using UnityEngine;

public class VentPointEDIT : Interactable
{
    [SerializeField] private Transform otherPoint;
    [SerializeField] private bool isEnterPoint;

    public override void InteractWith()
    {
        if (otherPoint == null) return;

        TeleportTo(otherPoint);

        if (isEnterPoint)
        {
            // Play the “enter vent” stinger
            RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
            // Tell the AmbientController we’re now in the vent
            AmbientController.Instance.SetInVent(true);
        }
        else
        {
            // Tell the AmbientController we’ve exited
            AmbientController.Instance.SetInVent(false);
            // Play the “exit vent” stinger
            RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
        }
    }

    private void TeleportTo(Transform targetPoint)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = targetPoint.position;

        if (cc != null) cc.enabled = true;
    }
}
