using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class VacuumSFX : MonoBehaviour
{
    [SerializeField] private EventReference vacuumSoundEvent;

    private EventInstance vacuumInstance;

    private void Awake()
    {
        vacuumInstance = RuntimeManager.CreateInstance(vacuumSoundEvent);
        RuntimeManager.AttachInstanceToGameObject(vacuumInstance, transform, GetComponent<Rigidbody>());
    }

    public void PlayVacuumSound()
    {
        vacuumInstance.start();
    }

    public void StopVacuumSound()
    {
        vacuumInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        vacuumInstance.release();
    }
}
