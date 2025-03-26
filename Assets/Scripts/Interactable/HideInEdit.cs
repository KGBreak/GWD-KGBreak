using UnityEngine;
using Util;

public class HideInEdit : Interactable
{
    private bool isInside = false;
    private GameObject player;
    private Vector3 exitPosition;
    [SerializeField] private ExitDirection[] legalExitDirections;
    private Renderer[] playerRenderers;
    private Collider playerCollider;
    private PlayerMovement playerMovement;
    private Camera playerCamera;

    //variables for passages
    [SerializeField]
    private bool isPassage;
    [SerializeField]
    private Transform otherPoint;
    [SerializeField]
    private bool isEnterPoint;

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

        player = GameObject.FindGameObjectWithTag("Player");
        playerCamera = Camera.main; // Get the main camera

        // Get all Renderers from child objects
        playerRenderers = player.GetComponentsInChildren<Renderer>();
        playerCollider = player.GetComponent<Collider>();
        playerMovement = player.GetComponent<PlayerMovement>();

    }

    void EnterObject()
    {
        if (player == null) return;

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

    void ExitObject()
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


    }


    Vector3 GetBestExitDirection()
    {
        if (legalExitDirections.Length == 0) return transform.right; // Default to right if none are set

        Vector3 cameraViewDir = playerCamera.transform.forward; // Use camera's view direction
        Vector3 bestDirection = Vector3.zero;
        float maxDot = -1f; // Lowest possible dot product

        foreach (ExitDirection dir in legalExitDirections)
        {
            Vector3 worldDirection = DirectionHelper.GetWorldDirection(dir);
            float dotProduct = Vector3.Dot(cameraViewDir, worldDirection);

            if (dotProduct > maxDot) // Closer to player's camera direction
            {
                maxDot = dotProduct;
                bestDirection = worldDirection;
            }
        }

        return bestDirection;
    }

    //Used for passages
    private void TeleportTo(Transform targetPoint)
    {
        if (targetPoint == null)
        {
            return;
        }

        //Vector3 offset = targetPoint.forward * (GetComponent<Renderer>().bounds.size.z + 0.5f);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterController characterController = player.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.enabled = false; // Disable the CharacterController to set the position directly
                player.transform.position = targetPoint.position; //+ offset;
                characterController.enabled = true; // Re-enable the CharacterController
            }
            else
            {
                player.transform.position = targetPoint.position; //+ offset;
            }
        }
    }
}




