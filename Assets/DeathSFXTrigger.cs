using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DeathSFXTrigger : MonoBehaviour
{
    [SerializeField]
    private EventReference soundEvent;

    private EventInstance soundInstance;

    public void PlaySound()
    {
        if (!soundInstance.isValid())
            soundInstance = RuntimeManager.CreateInstance(soundEvent);

        soundInstance.start();
    }

    public void StopSound()
    {
        if (soundInstance.isValid())
        {
            soundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            soundInstance.release();
            soundInstance.clearHandle();
        }
    }
}
