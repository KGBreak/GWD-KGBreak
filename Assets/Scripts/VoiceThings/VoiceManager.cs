using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using Util;
using System.Linq;


public class VoiceManager : MonoBehaviour
{
    [SerializeField] private List<Dialog> fillerDialogsList = new List<Dialog>();
    [SerializeField] private List<VoiceLine> investigateLines = new List<VoiceLine>();
    [SerializeField] private Transform playerTransform;

    private Queue<Dialog> fillerDialogs;
    private HashSet<Transform> investigatingEnemies = new HashSet<Transform>();

    private Queue<Dialog> dialogQueue = new Queue<Dialog>();
    private VoiceRoom currentRoom;
    private Coroutine currentDialogCoroutine;
    private Dialog currentDialog;

    private float checkInterval = 5f;
    private float nextCheckTime = 0f;

    private void Awake()
    {
        fillerDialogs = new Queue<Dialog>(fillerDialogsList);
    }

private void Update()
    {
        // Run update every 5 seconds
        if (Time.time < nextCheckTime || investigatingEnemies.Count > 0) return;
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
                Debug.Log("play one");
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

        if (dialog.isImportant)
        {
            // stop any running coroutine + clear all queued stuff
            if (currentDialogCoroutine != null) StopCoroutine(currentDialogCoroutine);
            currentDialogCoroutine = null;
            dialogQueue.Clear();

            // immediately start playing the important dialog
            currentDialogCoroutine = StartCoroutine(PlayDialog(dialog));
            return;
        }

        // non-important dialogs can still be queued…
        dialogQueue.Enqueue(dialog);
    }


    // Called from voice room. Set room to null, if dialog is playing and is not important and not intercom, end it.
    public void OnPlayerExitRoom(VoiceRoom room)
    {
        if (currentRoom == room)
        {

            currentRoom = null;
            dialogQueue.Clear();

            if (currentDialog != null && !currentDialog.isImportant && !currentDialog.actors.Contains(VoiceActor.Intercom))
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

        bool isIntercomOnly = dialog.actors.All(a => a == VoiceActor.Intercom);

        List<EventInstance> playingInstances = new List<EventInstance>();

        foreach (var line in dialog.voiceLines)
        {
            EventInstance instance = RuntimeManager.CreateInstance(line.eventRef);
            Transform speakerTransform = GetNpcTransformByActor(line.actor);

            if (line.actor == VoiceActor.Intercom)
            {
                RuntimeManager.AttachInstanceToGameObject(instance, playerTransform, playerTransform.GetComponent<Rigidbody>());
            }
            else if (speakerTransform != null)
            {
                RuntimeManager.AttachInstanceToGameObject(instance, speakerTransform, speakerTransform.GetComponent<Rigidbody>());
            }

            instance.start();
            playingInstances.Add(instance);
        }

        // Wait for all lines to finish
        bool allStopped = false;
        while (!allStopped)
        {
            allStopped = true;
            foreach (var instance in playingInstances)
            {
                instance.getPlaybackState(out var state);
                if (state != PLAYBACK_STATE.STOPPED)
                {
                    allStopped = false;

                    if (investigatingEnemies.Count > 0 && !isIntercomOnly)
                    {
                        instance.setPaused(true);
                    }
                    else
                    {
                        instance.setPaused(false);
                    }
                }
            }
            yield return null;
        }

        // Release all instances
        foreach (var instance in playingInstances)
        {
            instance.release();
        }

        currentDialogCoroutine = null;
        currentDialog = null;
    }



    private Transform GetNpcTransformByActor(VoiceActor actor)
    {
        if (actor == VoiceActor.Intercom) return null;

        foreach (var npc in currentRoom.GetNpcs())
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
        foreach (var npc in currentRoom.GetNpcs())
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
                fillerDialogs.Enqueue(dialog); // No deleting for now
                return dialog; // Found a valid one
            }
            else
            {
                fillerDialogs.Enqueue(dialog); // Put it back at the end
            }
        }

        return null; // No valid filler found
    }

    public void playInvestigatingLines(Transform transform, VoiceActor voiceActor, InvestigatingState investigateState)
    {
        investigatingEnemies.Add(transform);

        foreach (VoiceLine voiceLine in investigateLines)
        {
            if (voiceLine.actor == voiceActor && voiceLine.investigatingState == investigateState)
            {
                // create & attach
                var inst = RuntimeManager.CreateInstance(voiceLine.eventRef);
                if (transform != null)
                    RuntimeManager.AttachInstanceToGameObject(inst, transform, transform.GetComponent<Rigidbody>());

                // play and immediately release (FMOD will keep it alive until it’s done)
                inst.start();
                inst.release();
                break;
            }
        }

    }

    public void stopInvestigatingLines(Transform transform, VoiceActor voiceActor)
    {
        investigatingEnemies.Remove(transform);
        foreach (VoiceLine voiceLine in investigateLines)
        {
            if (voiceLine.actor == voiceActor && voiceLine.investigatingState == InvestigatingState.None)
            {
                // create & attach
                var inst = RuntimeManager.CreateInstance(voiceLine.eventRef);
                if (transform != null)
                    RuntimeManager.AttachInstanceToGameObject(inst, transform, transform.GetComponent<Rigidbody>());

                // play and immediately release (FMOD will keep it alive until it’s done)
                inst.start();
                inst.release();
                break;
            }
        }
    }

    public void removeInvestigator(Transform transform)
    {
        investigatingEnemies.Remove(transform);
    }
}
