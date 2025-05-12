using UnityEngine;

public class ConsumeLore : Interactable
{
    [SerializeField] private string loreDoc;
    [SerializeField] private int entryNumber;

    public override void InteractWith()
    {
        // Get reference to the player object (assuming it's tagged as "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Get the LoreManager component from the player object
            LoreManager loreManager = player.GetComponent<LoreManager>();

            if (loreManager != null)
            {
                // Add lore entry to the player's lore manager
                loreManager.AddLoreEntry(loreDoc, entryNumber, this.gameObject);

                // Destroy this object after interaction
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("LoreManager not found on Player.");
            }
        }
        else
        {
            Debug.LogWarning("Player object not found.");
        }
    }
}

