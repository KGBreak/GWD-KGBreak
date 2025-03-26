using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Objects/Settings")]
public class Settings : ScriptableObject
{
    [Range(0, 1f)]
    public float masterVolume;
    [Range(0, 1f)]
    public float sfxVolume;
    [Range(0, 1f)]
    public float musicVolume;
    [Range(0, 1f)]
    public float mouseSensetivity;
}
