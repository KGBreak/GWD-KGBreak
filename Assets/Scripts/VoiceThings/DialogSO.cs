using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Util;

[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable Objects/Dialog")]
public class Dialog : ScriptableObject
{
    public List<VoiceLine> voiceLines;
    public bool isImportant = false;
    public List<VoiceActor> actors;
    public bool wasPlayed;
      
}
