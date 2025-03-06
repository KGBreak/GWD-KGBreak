using UnityEngine;

public class HideIn : Interactable
{
    private bool isInside = false;
    private GameObject player;
    private Vector3 exitPosition;
    private Renderer[] playerRenderers;
    private Collider playerCollider;
    private PlayerMovement playerMovement;

    public override void InteractWith()
    {
        if (isInside)
        {
            ExitObject();
        }
        else
        {
            EnterObject();
        }
    }

    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure it's tagged as 'Player'.");
            return;
        }

        // Get all Renderers from child objects
        playerRenderers = player.GetComponentsInChildren<Renderer>();
        playerCollider = player.GetComponent<Collider>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void EnterObject()
    {
        if (player == null) return;

        // Store exit position slightly outside the object
        exitPosition = transform.position + transform.right * 2f; // Adjust offset as needed

        // Hide player by disabling all Renderers
        foreach (Renderer rend in playerRenderers)
        {
            rend.enabled = false;
        }

        // Disable collider to avoid interactions
        if (playerCollider) playerCollider.enabled = false;
        if (playerMovement) playerMovement.SetHiding(true);


        // Move player inside the object
        player.transform.position = transform.position;

        isInside = true;
    }

    void ExitObject()
    {
        if (player == null) return;

        // Move player back outside
        player.transform.position = exitPosition;

        // Show player by enabling all Renderers again
        foreach (Renderer rend in playerRenderers)
        {
            rend.enabled = true;
        }

        // Re-enable collider
        if (playerCollider) playerCollider.enabled = true;
        if (playerMovement) playerMovement.SetHiding(false);

        isInside = false;
    }
}


