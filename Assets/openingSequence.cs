using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class openingSequence : MonoBehaviour
{
    [SerializeField] private GameObject openingNpc1;
    [SerializeField] private GameObject openingNpc2;
    [SerializeField] private GameObject openingVentalation;
    [SerializeField] private VoiceLine gibberish;
    [SerializeField] private VoiceLine transition;
    [SerializeField] private Dialog openingConvo;
    [SerializeField] private GameObject openingDoor;

    private StateMachine Npc1SM;
    private StateMachine Npc2SM;
    private VentPoint ventPoint;
    private Door door;

    private EventInstance _currentInstance;

    private bool hasConsumed = false;
    private bool hasEntered = false;
    void Start()
    {
        Npc1SM = openingNpc1.GetComponent<StateMachine>();
        Npc2SM = openingNpc2.GetComponent<StateMachine>();
        ventPoint = openingVentalation.GetComponent<VentPoint>();
        door = openingDoor.GetComponent<Door>();

        _currentInstance = RuntimeManager.CreateInstance(gibberish.eventRef);
        _currentInstance.start();

    }

    public void consumeDictionary()
    {
        if (hasConsumed) return;
        hasConsumed = true;
        _currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _currentInstance.release();

        _currentInstance = RuntimeManager.CreateInstance(transition.eventRef);
        _currentInstance.start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasEntered || !other.CompareTag("Player")) return;
        hasEntered = true;
        StartCoroutine(PlayDialog());
    }

    private IEnumerator PlayDialog()
    {
        // 1) Wait for the previous instance (gibberish or transition) to finish
        PLAYBACK_STATE state;
        do
        {
            _currentInstance.getPlaybackState(out state);
            yield return null;
        }
        while (state == PLAYBACK_STATE.PLAYING);

        // 2) Play every VoiceLine in openingConvo
        var instances = new List<EventInstance>();
        foreach (var line in openingConvo.voiceLines)  // voiceLines list from your DialogSO asset :contentReference[oaicite:0]{index=0}
        {
            var inst = RuntimeManager.CreateInstance(line.eventRef);
            // Attach to NPC or player/intercom
            Transform attachTo = GetNpcTransform(line.actor);
            if (attachTo != null)
            {
                RuntimeManager.AttachInstanceToGameObject(inst, attachTo, attachTo.GetComponent<Rigidbody>());
            }
            else if (line.actor == VoiceActor.Intercom)
            {
                var player = GameObject.FindGameObjectWithTag("Player").transform;
                RuntimeManager.AttachInstanceToGameObject(inst, player, player.GetComponent<Rigidbody>());
            }

            inst.start();
            inst.release();       // allow FMOD to clean up once playback finishes :contentReference[oaicite:1]{index=1}
            instances.Add(inst);
        }

        // 3) Wait until all these dialog instances have stopped
        bool allDone;
        do
        {
            allDone = true;
            foreach (var inst in instances)
            {
                inst.getPlaybackState(out state);
                if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
                {
                    allDone = false;
                    break;
                }
            }
            yield return null;
        }
        while (!allDone);


        Npc1SM.enabled = true;
        Npc2SM.enabled = true;

        yield return new WaitForSeconds(3f);

        // 4) Now that dialog is fully over, run your completion handler
        OnDialogComplete();
    }

    private void OnDialogComplete()
    {
        ventPoint.enabled = true;

        door.InteractWith();

        StartCoroutine(DisableDoorAfterDelay(2f));  // wait 2 seconds
    }

    private IEnumerator DisableDoorAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        door.enabled = false;
    }

    // helper to map a VoiceActor to one of your two NPC GameObjects
    private Transform GetNpcTransform(VoiceActor actor)
    {
        var comp1 = openingNpc1.GetComponent<NPCVoiceActor>();
        if (comp1 != null && comp1.actor == actor) return openingNpc1.transform;

        var comp2 = openingNpc2.GetComponent<NPCVoiceActor>();
        if (comp2 != null && comp2.actor == actor) return openingNpc2.transform;

        return null;
    }


}
