using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class DetectionMeter : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Camera mainCamera;

    [Header("FMOD Settings")]
    [SerializeField] private EventReference detectionBeepEvent;
    [SerializeField] private EventReference detectedFullEvent;

    private float beepTimer = 0f;
    private float currentBeepInterval = 0.8f;
    private bool fullDetectionTriggered = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (slider.value == 0f)
        {
            slider.gameObject.SetActive(false);
            beepTimer = 0f;
            fullDetectionTriggered = false;
        }
        else
        {
            slider.gameObject.SetActive(true);
            slider.transform.rotation = mainCamera.transform.rotation;

            if (beepTimer == 0f)
            {
                RuntimeManager.PlayOneShot(detectionBeepEvent); // First beep immediately
            }

            currentBeepInterval = Mathf.Lerp(0.8f, 0.15f, slider.value * slider.value);
            beepTimer += Time.deltaTime;

            if (beepTimer >= currentBeepInterval)
            {
                RuntimeManager.PlayOneShot(detectionBeepEvent);
                beepTimer = 0f;
            }

            if (slider.value >= 1f && !fullDetectionTriggered)
            {
                RuntimeManager.PlayOneShot(detectedFullEvent);
                fullDetectionTriggered = true;
            }
        }
    }


    public void UpdateMeter(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
}

