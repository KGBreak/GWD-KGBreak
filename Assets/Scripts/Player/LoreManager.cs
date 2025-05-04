using System;
using System.Collections.Generic;
using UnityEngine;

public class LoreManager : MonoBehaviour
{
    public static event Action<int> OnLoreEntryAdded;

    public static Dictionary<int, (string loreDoc, GameObject loreObject)> loreEntries = new Dictionary<int, (string, GameObject)>();

    public void AddLoreEntry(string loreDoc, int entryNumber, GameObject loreObject)
    {
        loreEntries.Add(entryNumber, (loreDoc, loreObject));
        Debug.Log($"Lore added: {loreDoc} (Entry {entryNumber})");

        OnLoreEntryAdded?.Invoke(entryNumber);
    }
}