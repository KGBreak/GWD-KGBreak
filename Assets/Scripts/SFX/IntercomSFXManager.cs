using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class IntercomSFXManager : MonoBehaviour
{
    [Header("FMOD events")]
    [SerializeField] EventReference whoosh2D;   // 2‑D whoosh
    [SerializeField] EventReference voice3D;    // 3‑D intercom VO

    [Header("Entry numbers for the two dictionary pages")]
    [SerializeField] int[] dictionaryEntryIDs = { 100, 101 };

    HashSet<int> collected = new HashSet<int>();
    bool fired = false;

    void Awake()
    {
        /* catch pages picked up BEFORE this script loaded */
        foreach (int id in dictionaryEntryIDs)
            if (LoreManager.loreEntries.ContainsKey(id))
                collected.Add(id);

        /* listen for future pickups */
        LoreManager.OnLoreEntryAdded += HandleLoreAdded;
    }

    void OnDestroy()            // tidy‑up the subscription
    {
        LoreManager.OnLoreEntryAdded -= HandleLoreAdded;
    }

    void HandleLoreAdded(int entryID)
    {
        if (fired) return;                      // already played

        foreach (int id in dictionaryEntryIDs)
            if (entryID == id)
                collected.Add(id);

        if (collected.Count == dictionaryEntryIDs.Length)
            PlayBothSounds();
    }

    void PlayBothSounds()
    {
        fired = true;

        // 1) 2‑D whoosh
        RuntimeManager.PlayOneShot(whoosh2D);

        // 2) 3‑D VO at this GameObject’s position
        var voiceInst = RuntimeManager.CreateInstance(voice3D);
        voiceInst.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
        voiceInst.start();
        voiceInst.release();
    }
}
