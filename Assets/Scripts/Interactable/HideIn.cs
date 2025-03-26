using UnityEngine;


public enum ExitDirection
{
    UP, DOWN, LEFT, RIGHT, FORWARD, BACKWARD

}

public class HideIn : Interactable
{
    private bool isInside = false;
    private GameObject player;
    private Vector3 exitPosition;
    [SerializeField] private ExitDirection[] legalExitDirections;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private bool canBringItem = false;
    private Renderer[] playerRenderers;
    private Collider playerCollider;
    private PlayerMovement playerMovement;
    private Camera playerCamera;
    private HidingManager hidingManager;

    public override void InteractWith()
    {
        if (isInside)
        {
            if (exitPoint == null) {
                ExitObject();
            }
            else
            {
                // Move to linkted object
                exitPoint.gameObject.GetComponent<HideIn>().InteractWith();
                isInside = false;
            }
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

        player = GameObject.FindGameObjectWithTag("Player");
        playerCamera = Camera.main; // Get the main camera

        // Get all Renderers from child objects
        playerRenderers = player.GetComponentsInChildren<Renderer>();
        playerCollider = player.GetComponent<Collider>();
        playerMovement = player.GetComponent<PlayerMovement>();
        hidingManager = player.GetComponent<HidingManager>();

    }

    void EnterObject()
    {
        if (player == null) return;
        if (!canBringItem)
        {
            player.GetComponent<ItemManager>().EjectCurrentItem();
        }
        hidingManager.SetHidingObject(this);

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
        player.transform.rotation = transform.rotation;

        isInside = true;
    }

    public void ExitObject()
    {
        if (player == null) return;

        Camera playerCamera = Camera.main; // Get the main camera
        Vector3 cameraViewDir = playerCamera.transform.forward; // Use camera's forward vector

        // Determine the best exit direction based on where the camera is looking
        Vector3 bestExitDirection = GetBestExitDirection();

        // Move player to the best exit position
        player.transform.position = transform.position + bestExitDirection;

        // Show player by enabling all Renderers again
        foreach (Renderer rend in playerRenderers)
        {
            rend.enabled = true;
        }

        // Re-enable collider
        if (playerCollider) playerCollider.enabled = true;
        if (playerMovement) playerMovement.SetHiding(false);

        isInside = false;
        hidingManager.SetHidingObject(null);


    }


    Vector3 GetBestExitDirection()
    {
        if (legalExitDirections.Length == 0) return transform.right; // Default to right if none are set

        Vector3 cameraViewDir = playerCamera.transform.forward; // Use camera's view direction
        Vector3 bestDirection = Vector3.zero;
        float maxDot = -1f; // Lowest possible dot product

        foreach (ExitDirection dir in legalExitDirections)
        {
            Vector3 worldDirection = GetWorldDirection(dir);
            float dotProduct = Vector3.Dot(cameraViewDir, worldDirection);

            if (dotProduct > maxDot) // Closer to player's camera direction
            {
                maxDot = dotProduct;
                bestDirection = worldDirection;
            }
        }

        return bestDirection;
    }

    Vector3 GetWorldDirection(ExitDirection direction)
    {
        switch (direction)
        {
            case ExitDirection.UP: return Vector3.up;         // Always global up
            case ExitDirection.DOWN: return Vector3.down;     // Always global down
            case ExitDirection.LEFT: return Vector3.left;     // Always global left
            case ExitDirection.RIGHT: return Vector3.right;   // Always global right
            case ExitDirection.FORWARD: return Vector3.forward; // Always global forward
            case ExitDirection.BACKWARD: return Vector3.back; // Always global backward
            default: return Vector3.right; // Default to world right
        }
    }
}


