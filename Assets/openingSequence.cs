using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util;

public class openingSequence : MonoBehaviour
{
    [SerializeField] private GameObject openingNpc1;
    [SerializeField] private GameObject openingNpc2;
    [SerializeField] private GameObject openingVentalationLook;
    [SerializeField] private GameObject openingVentalationEnter;
    [SerializeField] private VoiceLine gibberish;
    [SerializeField] private VoiceLine transition;
    [SerializeField] private Dialog openingConvo;
    [SerializeField] private VoiceLine interuptVoiceline;
    [SerializeField] private Dialog endingConvo;
    [SerializeField] private GameObject openingDoor;

    private StateMachine Npc1SM;
    private StateMachine Npc2SM;
    private VentPoint ventPoint;
    private HideIn ventPointLook;
    private Door door;

    private EventInstance _currentInstance;

    private bool hasConsumed = false;
    private bool hasEntered = false;
    void Start()
    {
        Npc1SM = openingNpc1.GetComponent<StateMachine>();
        Npc2SM = openingNpc2.GetComponent<StateMachine>();
        ventPointLook = openingVentalationLook.GetComponent<HideIn>();
        ventPoint = openingVentalationEnter.GetComponent<VentPoint>();
        door = openingDoor.GetComponent<Door>();

        _currentInstance = RuntimeManager.CreateInstance(gibberish.eventRef);
        _currentInstance.start();
        door.InteractWith();

    }

    public void consumeDictionary()
    {
        if (hasConsumed) return;
        hasConsumed = true;

        ventPoint.enabled = true;
        _currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
        PLAYBACK_STATE state;

        // 1) Wait for whatever _currentInstance is (gibberish or transition) to finish
        do
        {
            _currentInstance.getPlaybackState(out state);
            yield return null;
        }
        while (state == PLAYBACK_STATE.PLAYING);

        // 2) Play the opening conversation
        yield return StartCoroutine(PlayConvo(openingConvo));

        // 3) Play the single interrupt VoiceLine
        var interruptInst = RuntimeManager.CreateInstance(interuptVoiceline.eventRef);
        AttachLine(interruptInst, interuptVoiceline.actor);
        interruptInst.start();
        interruptInst.release();
        // wait until it’s done
        do
        {
            interruptInst.getPlaybackState(out state);
            yield return null;
        }
        while (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING);

        // 4) Play the ending conversation
        yield return StartCoroutine(PlayConvo(endingConvo));

        // 5) Restore AI and open the door
        Npc1SM.enabled = true;
        Npc2SM.enabled = true;

        yield return new WaitForSeconds(4f);
        OnDialogComplete();
    }

    private void OnDialogComplete()
    {
        ventPointLook.enabled = true;

        door.InteractWith();

        StartCoroutine(DisableDoorAfterDelay(7f));
    }

    private IEnumerator DisableDoorAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        door.enabled = false;
        Destroy(openingNpc1);
    }

    /// <summary>
    /// Plays every VoiceLine in the given Dialog asset and waits until they’re all done.
    /// </summary>
    private IEnumerator PlayConvo(Dialog convo)
    {
        PLAYBACK_STATE state;
        var instances = new List<EventInstance>();

        foreach (var line in convo.voiceLines)
        {
            var inst = RuntimeManager.CreateInstance(line.eventRef);
            AttachLine(inst, line.actor);
            inst.start();
            inst.release();
            instances.Add(inst);
        }

        // wait until all lines are finished
        bool anyPlaying;
        do
        {
            anyPlaying = false;
            foreach (var inst in instances)
            {
                inst.getPlaybackState(out state);
                if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
                {
                    anyPlaying = true;
                    break;
                }
            }
            yield return null;
        }
        while (anyPlaying);
    }



    /// <summary>
    /// Attaches an FMOD instance to the proper transform (NPC or Player) based on actor.
    /// </summary>
    private void AttachLine(EventInstance inst, VoiceActor actor)
    {
        Transform target = GetNpcTransform(actor);
        if (target != null)
        {
            RuntimeManager.AttachInstanceToGameObject(inst, target, target.GetComponent<Rigidbody>());
        }
        else if (actor == VoiceActor.Intercom)
        {
            var player = GameObject.FindGameObjectWithTag("Player").transform;
            RuntimeManager.AttachInstanceToGameObject(inst, player, player.GetComponent<Rigidbody>());
        }
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
