using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using System.Collections;

public class IntercomSFXManager : MonoBehaviour
{
    [Header("FMOD events")]
    [SerializeField] EventReference whoosh2D;     // 2‑D SFX
    [SerializeField] EventReference voice3D;      // 3‑D VO
    [SerializeField] EventReference gibberishLoop;// 3‑D random chatter loop
    [SerializeField] EventReference EnglishLoop;
    [SerializeField] float EnglishLoopDelay = 1.0f;

    [Header("Dictionary pages needed")]
    [SerializeField] int[] dictionaryEntryIDs = { 100, 101 };

    HashSet<int> collected = new HashSet<int>();
    bool fired = false;

    EventInstance gibberishInst;
    EventInstance EnglishInst;// running loop

    void Awake()
    {
        /* catch pages found before this script existed */
        foreach (int id in dictionaryEntryIDs)
            if (LoreManager.loreEntries.ContainsKey(id))
                collected.Add(id);

        LoreManager.OnLoreEntryAdded += HandleLoreAdded;
    }

    void Start()
    {
        /* start the gibberish chatter immediately */
        gibberishInst = RuntimeManager.CreateInstance(gibberishLoop);
        gibberishInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        gibberishInst.start();
    }

    void OnDestroy()
    {
        LoreManager.OnLoreEntryAdded -= HandleLoreAdded;

        if (gibberishInst.isValid())
            gibberishInst.release();
    }

    void HandleLoreAdded(int entryID)
    {
        if (fired) return;

        foreach (int id in dictionaryEntryIDs)
            if (entryID == id)
                collected.Add(id);

        if (collected.Count == dictionaryEntryIDs.Length)
            PlayBothSounds();
    }

    void PlayBothSounds()
    {
        fired = true;

        /* fade out the gibberish chatter loop */
        gibberishInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        gibberishInst.release();

        /* play 2‑D whoosh */
        RuntimeManager.PlayOneShot(whoosh2D);

        /* play 3‑D transition VO */
        var voiceInst = RuntimeManager.CreateInstance(voice3D);
        voiceInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        voiceInst.start();
        voiceInst.release();

        /* 4) start coroutine that waits, then launches Russian loop */
        StartCoroutine(StartEnglishLoopAfterDelay());
    }
    IEnumerator StartEnglishLoopAfterDelay()
    {
        yield return new WaitForSeconds(EnglishLoopDelay);

        EnglishInst = RuntimeManager.CreateInstance(EnglishLoop);
        EnglishInst.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        EnglishInst.start();
    }

}
