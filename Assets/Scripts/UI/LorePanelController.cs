using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LorePanelController : MonoBehaviour
{
    public TMP_Text loreText; 
    public TMP_Text entryNumberText; 
    public Button leftArrow;
    public Button rightArrow;
    public ScrollRect scrollRect;
    public RectTransform contentRectTransform;

    private int currentEntryIndex = 0;
    private List<int> loreEntryKeys;

    void Start()
    {
        loreEntryKeys = new List<int>(LoreManager.loreEntries.Keys);
        DisplayLoreEntry(currentEntryIndex);

        leftArrow.onClick.AddListener(ShowPreviousEntry);
        rightArrow.onClick.AddListener(ShowNextEntry);
    }

    void OnEnable()
    {
        loreEntryKeys = new List<int>(LoreManager.loreEntries.Keys);
        loreEntryKeys.Sort();

        if (loreEntryKeys.Count > 0)
        {
            currentEntryIndex = loreEntryKeys.Count - 1;
            DisplayLoreEntry(currentEntryIndex);
        }
        else
        {
            loreText.text = "No lore entries yet.";
            entryNumberText.text = "";
        }
    }


    public void DisplayLoreEntry(int index)
    {
        loreEntryKeys = new List<int>(LoreManager.loreEntries.Keys);

        if (index >= 0 && index < loreEntryKeys.Count)
        {
            int entryNumber = loreEntryKeys[index];
            var entry = LoreManager.loreEntries[entryNumber];
            loreText.text = entry.loreDoc;
            entryNumberText.text = $"Entry {entryNumber}";

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
            scrollRect.verticalNormalizedPosition = 1f;
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