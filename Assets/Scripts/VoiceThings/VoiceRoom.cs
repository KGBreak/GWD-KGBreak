using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Util;
public class VoiceRoom : MonoBehaviour
{
    [SerializeField] private int roomId;
    [SerializeField] private List<Transform> npcs;
    [SerializeField] private Dialog specificDialog;
    [SerializeField] protected VoiceManager voiceManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (specificDialog != null && !specificDialog.wasPlayed)
            {
                voiceManager.OnPlayerEnterRoom(this, specificDialog);
                specificDialog.wasPlayed = true;
            }
            else
            {
                voiceManager.OnPlayerEnterRoom(this, null);
            }
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
