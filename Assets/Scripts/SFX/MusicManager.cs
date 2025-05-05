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
        _musicInst = RuntimeManager.CreateInstance(musicEvent);
        _musicInst.start();
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
