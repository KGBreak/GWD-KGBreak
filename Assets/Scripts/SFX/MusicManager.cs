using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private EventReference menuMusicEvent;

    private EventInstance musicInstance;

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(menuMusicEvent);
        musicInstance.start();
    }

    private void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}
