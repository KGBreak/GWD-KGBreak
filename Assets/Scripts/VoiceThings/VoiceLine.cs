using UnityEngine;
using Util;
using FMODUnity;

[CreateAssetMenu(fileName = "VoiceLine", menuName = "Scriptable Objects/VoiceLine")]
public class VoiceLine : ScriptableObject
{
    public VoiceActor actor;
    public EventReference eventRef;
    public InvestigatingState investigatingState;

}
