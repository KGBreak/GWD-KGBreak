using UnityEngine;
using Util;
using FMODUnity;

[CreateAssetMenu(fileName = "VoiceLine", menuName = "Scriptable Objects/VoiceLine")]
public class VoiceLine : ScriptableObject
{
    public VoiceActor actor;
    public EventReference eventRef;
    public float delayAfter = 0.3f;
    public int voiceLineID;
}
