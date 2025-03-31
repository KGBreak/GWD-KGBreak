using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LorePanelController : MonoBehaviour
{
    public RawImage loreImage;
    public Button leftArrow;
    public Button rightArrow;

    private int currentEntryIndex = 0;
    private List<int> loreEntryKeys;

    void Start()
    {
        loreEntryKeys = new List<int>(LoreManager.loreEntries.Keys);
        DisplayLoreEntry(currentEntryIndex);

        leftArrow.onClick.AddListener(ShowPreviousEntry);
        rightArrow.onClick.AddListener(ShowNextEntry);
    }

    public void DisplayLoreEntry(int index)
    {
        if (index >= 0 && index < loreEntryKeys.Count)
        {
            int entryNumber = loreEntryKeys[index];
            var entry = LoreManager.loreEntries[entryNumber];
            loreImage.texture = entry.loreObject.GetComponent<Renderer>().material.mainTexture as Texture2D;
        }
    }

    public void ShowPreviousEntry()
    {
        if (currentEntryIndex > 0)
        {
            currentEntryIndex--;
            DisplayLoreEntry(currentEntryIndex);
        }
    }

    public void ShowNextEntry()
    {
        if (currentEntryIndex < loreEntryKeys.Count - 1)
        {
            currentEntryIndex++;
            DisplayLoreEntry(currentEntryIndex);
        }
    }
}