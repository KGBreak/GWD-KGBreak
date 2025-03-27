using UnityEngine;
using UnityEngine.UI;

public class DetectionMeter : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Camera camera;

    // Update is called once per frame
    void Update()
    {
        if (slider.value == 0f)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            slider.gameObject.SetActive(true);
            slider.transform.rotation = camera.transform.rotation;
        }
    }

    public void UpdateMeter(float currentValue, float maxValue)
    {
        Debug.Log("Current value: " + currentValue + " Max value: " + maxValue + "procentage: " + currentValue / maxValue);
        slider.value = currentValue/maxValue;
    }
}
