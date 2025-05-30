using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    [Tooltip("FMOD event to play on scene start")]
    [SerializeField] private EventReference musicEvent;

    private EventInstance _musicInst;

    void Start()
    {
        if (!musicEvent.IsNull)
        {
            _musicInst = RuntimeManager.CreateInstance(musicEvent);
            _musicInst.start();
        }
        else
        {
            Debug.LogWarning($"[{nameof(MusicManager)}] No FMOD event assigned on “{gameObject.name}” – skipping music.");
        }
    }

    void OnDestroy()
    {
        if (_musicInst.isValid())
        {
            _musicInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInst.release();
            _musicInst.clearHandle();
        }
    }
}
