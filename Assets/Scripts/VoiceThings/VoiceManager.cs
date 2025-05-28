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
    private List<EventInstance> playingInstances = new List<EventInstance>();
    private List<EventInstance> investigatingInstances = new List<EventInstance>();

    private Queue<Dialog> dialogQueue = new Queue<Dialog>();
    private VoiceRoom currentRoom;
    private Coroutine currentDialogCoroutine;
    private Dialog currentDialog;

    private float checkInterval = 5f;
    private float nextCheckTime = 0f;

    private void Awake()
    {
        fillerDialogsList = fillerDialogsList
        .OrderBy(_ => UnityEngine.Random.value)
        .ToList();
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
        // ———> 1) As soon as you enter ANY new room, abort any
        //       ongoing interpersonal (non-important, non-intercom) dialog:
       /* if (currentDialog != null
            && !currentDialog.isImportant
            && !currentDialog.actors.Contains(VoiceActor.Intercom))
        {
            EndDialog();
        }*/

        // ———> 2) Now set the new room
        currentRoom = room;

        if (dialog == null)
            return;

        // ———> 3) If it’s an important dialog, abort anything else
        //       and start it immediately:
        if (dialog.isImportant)
        {
            EndDialog();
            dialogQueue.Enqueue(dialog);
            return;
        }

        // ———> 4) Otherwise it’s non-important: queue it up
        dialogQueue.Enqueue(dialog);

    }


    // Called from voice room. Set room to null, if dialog is playing and is not important and not intercom, end it.
    public void OnPlayerExitRoom(VoiceRoom room)
    {
        if (currentRoom == room)
        {

            currentRoom = null;
            dialogQueue.Clear();
        }
    }

    private IEnumerator PlayDialog(Dialog dialog)
    {
        currentDialog = dialog;

        bool isIntercomOnly = dialog.actors.All(a => a == VoiceActor.Intercom);

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
            CleanupInvestigationInstances();

            foreach (var instance in playingInstances)
            {
                instance.getPlaybackState(out var state);
                if (state != PLAYBACK_STATE.STOPPED)
                {
                    allStopped = false;

                    if ((investigatingEnemies.Count > 0 || investigatingInstances.Count > 0) && !isIntercomOnly)
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

        EndDialog();
    }

    public void EndDialog()
    {
        // 1) Stop coroutine
        if (currentDialogCoroutine != null)
        {
            StopCoroutine(currentDialogCoroutine);
            currentDialogCoroutine = null;
        }

        // 2) Stop & release every FMOD instance
        foreach (var inst in playingInstances)
        {
            inst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);  // or ALLOWFADEOUT
            inst.release();
        }
        playingInstances.Clear();

        // 3) Reset dialog state
        currentDialog = null;
        dialogQueue.Clear();
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
                investigatingInstances.Add(inst);
                break;
            }
        }

    }

    public void stopInvestigatingLines(Transform transform, VoiceActor voiceActor)
    {
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
                investigatingInstances.Add(inst);
                investigatingEnemies.Remove(transform);
                break;
            }
        }
    }

    public void removeInvestigator(Transform transform)
    {
        investigatingEnemies.Remove(transform);
    }

    private void CleanupInvestigationInstances()
    {
        for (int i = investigatingInstances.Count - 1; i >= 0; i--)
        {
            var inst = investigatingInstances[i];
            inst.getPlaybackState(out var state);
            if (state == PLAYBACK_STATE.STOPPED)
            {
                inst.release();
                investigatingInstances.RemoveAt(i);
            }
        }
    }
}
