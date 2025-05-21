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
    private Dictionary<VoiceActor, Transform> actorToTransform = new Dictionary<VoiceActor, Transform>();

    private void Update()
    {
        if (currentDialogCoroutine == null && currentRoom != null)
        {
            if (fillerDialogs.Count > 0)
            {
                dialogQueue.Enqueue(fillerDialogs.Dequeue());
            }

            if (dialogQueue.Count > 0)
            {
                currentDialogCoroutine = StartCoroutine(PlayDialog(dialogQueue.Dequeue()));
            }
        }
    }

    public void OnPlayerEnterRoom(VoiceRoom room, Dialog dialog)
    {
        if(dialog != null)
        {
            if (dialog.priority == Priority.Important && currentDialogCoroutine != null && currentDialog.priority != Priority.Important)
            {
                StopCoroutine(currentDialogCoroutine);
                currentDialogCoroutine = null;
                currentDialog = null;
                dialogQueue.Clear();
                dialogQueue.Enqueue(dialog);

            }
            else
            {
                dialogQueue.Enqueue(dialog);
            }

        }
        currentRoom = room;
    }

    public void OnPlayerExitRoom(VoiceRoom room)
    {
        if (currentRoom == room)
        {

            currentRoom = null;
            actorToTransform.Clear();

            if (currentDialog != null && currentDialog.priority != Priority.Important)
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
        if (actorToTransform.Count == 0)
        {
            // Assign npcs a voice actor
            var availableTransforms = new List<Transform>(currentRoom.getNpcs());

            foreach (var actor in dialog.actors)
            {
                var chosenTransform = availableTransforms[0];
                availableTransforms.RemoveAt(0);
                actorToTransform[actor] = chosenTransform;
            }
        }

        foreach (var line in dialog.voiceLines)
        {

            EventInstance instance = RuntimeManager.CreateInstance(line.eventRef);

            if (actorToTransform.TryGetValue(line.actor, out var speakerTransform))
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
}
