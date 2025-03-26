using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private Settings settings;

    [SerializeField]
    private Slider masterVolumeSlider;
    [SerializeField]
    private Slider sfxVolumeSlider;
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider mouseSensitivitySlider;

    void Start()
    {
        // Initialize sliders with current settings values
        masterVolumeSlider.value = settings.masterVolume;
        sfxVolumeSlider.value = settings.sfxVolume;
        musicVolumeSlider.value = settings.musicVolume;
        mouseSensitivitySlider.value = settings.mouseSensetivity;

        // Add listeners to update settings when sliders are changed
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
    }

    public void SetMasterVolume(float value)
    {
        settings.masterVolume = value;
    }

    public void SetSFXVolume(float value)
    {
        settings.sfxVolume = value;
    }

    public void SetMusicVolume(float value)
    {
        settings.musicVolume = value;
    }

    public void SetMouseSensitivity(float value)
    {
        settings.mouseSensetivity = value;
    }
}