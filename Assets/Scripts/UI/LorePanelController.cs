using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this namespace for TextMeshPro

public class LorePanelController : MonoBehaviour
{
    public TMP_Text loreText; // Reference to the TMP_Text component for lore content
    public TMP_Text entryNumberText; // Reference to the TMP_Text component for entry number
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
            loreText.text = entry.loreDoc;
            entryNumberText.text = $"Entry {entryNumber}";
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