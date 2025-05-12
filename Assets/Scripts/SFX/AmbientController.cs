using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbientController : MonoBehaviour
{
    public static AmbientController Instance { get; private set; }

    // --- Ambience event & parameter ---
    private const string AmbienceEventPath = "event:/Ambience/Ambience";
    private EventInstance _ambienceInst;
    private PARAMETER_ID _stateParamId;

    // --- Your existing snapshot ---
    private const string AirductSnapshotPath = "snapshot:/Airduct_Lowpass";
    private EventInstance _airductSnapshotInst;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create & start ambience
        _ambienceInst = RuntimeManager.CreateInstance(AmbienceEventPath);

        // Cache the “State” parameter ID
        var desc = RuntimeManager.GetEventDescription(AmbienceEventPath);
        if (desc.getParameterDescriptionByName("State", out var param) == FMOD.RESULT.OK)
            _stateParamId = param.id;
        else
            Debug.LogError("[AmbientController] ‘State’ param not found on Ambience");

        _ambienceInst.start();
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == "Asger_Greybox2")   // use build index if you prefer
                StartAmbience();
        };

    }

    /// <summary>
    /// Call true when entering the vent, false on exit.
    /// Drives both the ambience parameter and your Airduct_Lowpass snapshot.
    /// </summary>
    public void SetInVent(bool inVent)
    {
        // 1) Switch ambience parameter
        _ambienceInst.setParameterByID(_stateParamId, inVent ? 1f : 0f);

        // 2) Engage/disengage your low-pass snapshot
        if (inVent)
        {
            _airductSnapshotInst = RuntimeManager.CreateInstance(AirductSnapshotPath);
            _airductSnapshotInst.start();
        }
        else if (_airductSnapshotInst.isValid())
        {
            _airductSnapshotInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _airductSnapshotInst.release();
            _airductSnapshotInst.clearHandle();
        }
    }
    public void StartAmbience()
    {
        // recreate only if the handle is gone or no longer playing
        if (_ambienceInst.isValid())
        {
            FMOD.Studio.PLAYBACK_STATE state;
            _ambienceInst.getPlaybackState(out state);
            if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
                return;                     // already running
        }

        _ambienceInst = RuntimeManager.CreateInstance(AmbienceEventPath);
        _ambienceInst.setParameterByID(_stateParamId, 0f); // default “outside vent”
        _ambienceInst.start();
    }

    public void StopAmbience(bool fade = true)
    {
        if (!_ambienceInst.isValid()) return;
        _ambienceInst.stop(fade ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        _ambienceInst.release();
        _ambienceInst.clearHandle();        // mark as invalid so StartAmbience() knows to recreate
    }


    void OnDestroy()
    {
        _ambienceInst.release();
        if (_airductSnapshotInst.isValid())
            _airductSnapshotInst.release();
    }
}
