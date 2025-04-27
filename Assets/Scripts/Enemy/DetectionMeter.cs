using UnityEngine;
using UnityEngine.UI;

public class DetectionMeter : MonoBehaviour
{
    [SerializeField] Slider slider;
    Camera mainCamera;

    // Update is called once per frame
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (slider.value == 0f)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
            slider.transform.rotation = mainCamera.transform.rotation;
        }
    }

    public void UpdateMeter(float currentValue, float maxValue)
    {
        slider.value = currentValue/maxValue;
    }
}
