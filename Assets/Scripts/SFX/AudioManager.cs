using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("FMOD Events")]
    [SerializeField] EventReference menuMusicEvent;
    [SerializeField] EventReference levelMusicEvent;
    [SerializeField] EventReference ambienceEvent;        // your “event:/Ambience/Ambience”
    [SerializeField] string menuSceneName = "StartMenu";
    [SerializeField] string levelSceneName = "Asger_Greybox2";

    // handles for each playing instance
    EventInstance _menuInst;
    EventInstance _levelInst;
    EventInstance _ambienceInst;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1) kill *everything* on the master bus
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        // 2) clean up any old instances
        Cleanup(ref _menuInst);
        Cleanup(ref _levelInst);
        Cleanup(ref _ambienceInst);

        // 3) start the right audio for this scene
        if (scene.name == menuSceneName)
        {
            _menuInst = RuntimeManager.CreateInstance(menuMusicEvent);
            _menuInst.start();
        }
        else if (scene.name == levelSceneName)
        {
            _levelInst = RuntimeManager.CreateInstance(levelMusicEvent);
            _ambienceInst = RuntimeManager.CreateInstance(ambienceEvent);
            _levelInst.start();
            _ambienceInst.start();
        }
    }

    void Cleanup(ref EventInstance inst)
    {
        if (inst.isValid())
        {
            inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            inst.release();
            inst.clearHandle();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Cleanup(ref _menuInst);
        Cleanup(ref _levelInst);
        Cleanup(ref _ambienceInst);
    }
}
