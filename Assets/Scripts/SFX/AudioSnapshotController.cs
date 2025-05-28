using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioSnapshotController : MonoBehaviour
{
    public static AudioSnapshotController Instance { get; private set; }

    [SerializeField] private string snapshotPath = "snapshot:/Airduct_Lowpass";
    private EventInstance snapshotInstance;
    private bool isActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartSnapshot()
    {
        if (isActive) return;

        snapshotInstance = RuntimeManager.CreateInstance(snapshotPath);
        snapshotInstance.start();
        isActive = true;

        if (AmbientController.Instance != null)
        {
            AmbientController.Instance.SetInVent(true);
        }
    }

    public void StopSnapshot()
    {
        if (!isActive) return;

        snapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        snapshotInstance.release();
        isActive = false;

        if (AmbientController.Instance != null)
        {
            AmbientController.Instance.SetInVent(false);
        }
    }

    public bool IsSnapshotActive()
    {
        return isActive;
    }
}
