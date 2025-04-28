using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class VentExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (VentPointEDIT.AirDuctLoopInstance.isValid())
            {
                VentPointEDIT.AirDuctLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                VentPointEDIT.AirDuctLoopInstance.release();
            }
        }
    }
}
