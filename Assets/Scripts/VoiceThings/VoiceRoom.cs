using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Util;
[CreateAssetMenu(fileName = "VoiceLine", menuName = "Scriptable Objects/VoiceLine")]
public class VoiceRoom : MonoBehaviour
{
    [SerializeField] private int roomId;
    [SerializeField] private List<Transform> npcs;
    [SerializeField] private Dialog specificDialog;
    [SerializeField] private VoiceManager voiceManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            voiceManager.OnPlayerEnterRoom(this, specificDialog);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            voiceManager.OnPlayerExitRoom(this);
        }
    }

    public List<Transform> GetNpcs() { return npcs; }
}
