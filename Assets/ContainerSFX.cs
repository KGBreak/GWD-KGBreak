using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ContainerSFX : MonoBehaviour
{
    [SerializeField]
    private EventReference soundEvent;

    private EventInstance soundInstance;

    public void PlaySound()
    {
        if (!soundInstance.isValid())
        {
            soundInstance = RuntimeManager.CreateInstance(soundEvent);
            RuntimeManager.AttachInstanceToGameObject(soundInstance, transform, GetComponent<Rigidbody>()); // Rigidbody is optional
        }

        soundInstance.start();
    }

    private void OnDestroy()
    {
        if (soundInstance.isValid())
        {
            soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            soundInstance.release();
        }
    }
}
