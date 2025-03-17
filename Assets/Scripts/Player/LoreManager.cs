using System.Collections.Generic;
using UnityEngine;

public class LoreManager : MonoBehaviour
{
    private Dictionary<int, string> loreEntries = new Dictionary<int, string>();


    public void AddLoreEntry(string loreDoc, int entryNumber)
    {
        loreEntries.Add(entryNumber, loreDoc);
        Debug.Log($"Lore added: {loreDoc} (Entry {entryNumber})");
    }
}

