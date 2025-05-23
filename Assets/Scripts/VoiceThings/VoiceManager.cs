using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using Util;


public class VoiceManager : MonoBehaviour
{
    [SerializeField] private Queue<Dialog> fillerDialogs;


    private Queue<Dialog> dialogQueue = new Queue<Dialog>();
    private VoiceRoom currentRoom;
    private Coroutine currentDialogCoroutine;
    private Dialog currentDialog;

    private float checkInterval = 5f;
    private float nextCheckTime = 0f;


    private void Update()
    {
        // Run update every 5 seconds
        if (Time.time < nextCheckTime) return;
        nextCheckTime = Time.time + checkInterval;

        // If we are not running a dialog, we are in a room, and we can find a valid dialog from filler dialog for the voice actors in the room, then start filler dialog
        if (currentDialogCoroutine == null && currentRoom != null)
        {
            if (fillerDialogs.Count > 0)
            {
                var filler = GetValidFillerDialog();
                if (filler != null)
                {
                    dialogQueue.Enqueue(filler);
                }
            }

            if (dialogQueue.Count > 0)
            {
                currentDialogCoroutine = StartCoroutine(PlayDialog(dialogQueue.Dequeue()));
            }
        }
    }
    
    // Called from voice room, with optionally a dialog. 
    // If we are currently playing dialog and the given is important, then 
    public void OnPlayerEnterRoom(VoiceRoom room, Dialog dialog)
    {

        currentRoom = room;
        if (dialog == null) return;

        if (dialog.isImportant && currentDialogCoroutine != null) {
            StopCoroutine(currentDialogCoroutine);
            currentDialogCoroutine = null;
            currentDialog = null;
            dialogQueue.Clear();
        }

        dialogQueue.Enqueue(dialog);
    }
    
    // Called from voice room. Set room to null, if dialog is playing and is not important, end it.
    public void OnPlayerExitRoom(VoiceRoom room)
    {
        if (currentRoom == room)
        {

            currentRoom = null;

            if (currentDialog != null && !currentDialog.isImportant)
            {
                StopCoroutine(currentDialogCoroutine);
                currentDialogCoroutine = null;
                currentDialog = null;
            }
        }
    }

    private IEnumerator PlayDialog(Dialog dialog)
    {
        currentDialog = dialog;

        foreach (var line in dialog.voiceLines)
        {

            EventInstance instance = RuntimeManager.CreateInstance(line.eventRef);
            Transform speakerTransform = GetNpcTransformByActor(line.actor);
            if (speakerTransform != null)
            {
                RuntimeManager.AttachInstanceToGameObject(instance, speakerTransform, speakerTransform.GetComponent<Rigidbody>());
            }

            instance.start();

            PLAYBACK_STATE state;
            do
            {
                yield return null;
                instance.getPlaybackState(out state);
            }
            while (state != PLAYBACK_STATE.STOPPED);

            instance.release();
            yield return new WaitForSeconds(line.delayAfter);
        }

        currentDialogCoroutine = null;
        currentDialog = null;
    }

    private Transform GetNpcTransformByActor(VoiceActor actor)
    {
        if (actor == VoiceActor.Intercom) return null;

        foreach (var npc in currentRoom.getNpcs())
        {
            var voiceComponent = npc.GetComponent<NPCVoiceActor>();
            if (voiceComponent != null && voiceComponent.actor == actor)
            {
                return npc;
            }
        }

        Debug.LogWarning($"No NPC with actor {actor} found in room.");
        return null;
    }

    private Dialog GetValidFillerDialog()
    {
        // Get all VoiceActors present in the current room
        HashSet<VoiceActor> presentActors = new HashSet<VoiceActor>();
        foreach (var npc in currentRoom.getNpcs())
        {
            var voiceComponent = npc.GetComponent<NPCVoiceActor>();
            if (voiceComponent != null)
            {
                presentActors.Add(voiceComponent.actor);
            }
        }

        // Look through filler dialogs for one that only uses those actors
        int dialogCount = fillerDialogs.Count;
        for (int i = 0; i < dialogCount; i++)
        {
            Dialog dialog = fillerDialogs.Dequeue(); // Remove from queue to inspect

            bool allActorsPresent = true;
            foreach (var actor in dialog.actors)
            {
                if (!presentActors.Contains(actor) && actor != VoiceActor.Intercom)
                {
                    allActorsPresent = false;
                    break;
                }
            }

            if (allActorsPresent)
            {
                return dialog; // Found a valid one
            }
            else
            {
                fillerDialogs.Enqueue(dialog); // Put it back at the end
            }
        }

        return null; // No valid filler found
    }
}
