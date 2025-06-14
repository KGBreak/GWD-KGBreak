using UnityEngine;
using Util;
using FMODUnity;

public class HideIn : Interactable
{
    private bool isInside = false;
    private GameObject player;
    private Vector3 exitPosition;

    [SerializeField] private ExitDirection[] legalExitDirections;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private bool canBringItem = false;

    [SerializeField] private bool exitToFirstPerson = false;
    [SerializeField] private bool exitWithFilter = false;

    private Renderer[] playerRenderers;
    private Collider playerCollider;
    private PlayerMovement playerMovement;
    private Camera playerCamera;
    private HidingManager hidingManager;
    private CameraController camController;

    public override void InteractWith()
    {
        if (isInside)
        {
            ExitObject();
            RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
        }
        else
        {
            RuntimeManager.PlayOneShot("event:/Player/EnterMorph");
            EnterObject();
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCamera = Camera.main;
        playerRenderers = player.GetComponentsInChildren<Renderer>();
        playerCollider = player.GetComponent<Collider>();
        playerMovement = player.GetComponent<PlayerMovement>();
        hidingManager = player.GetComponent<HidingManager>();
        camController = player.GetComponentInChildren<CameraController>();
    }

    public void EnterObject()
    {
        if (player == null) return;

        if (!canBringItem)
        {
            player.GetComponent<ItemManager>().EjectCurrentItem();
        }

        hidingManager.SetHidingObject(this);

        foreach (Renderer rend in playerRenderers)
        {
            rend.enabled = false;
        }

        if (playerCollider) playerCollider.enabled = false;
        if (playerMovement) playerMovement.SetHiding(true);

        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;

        AudioSnapshotController.Instance.StartSnapshot();

        isInside = true;
    }

    public void ExitObject()
    {
        if (player == null) return;

        Vector3 bestExitDirection = GetBestExitDirection();
        player.transform.position = transform.position + bestExitDirection;

        foreach (Renderer rend in playerRenderers)
        {
            rend.enabled = true;
        }

        if (playerCollider) playerCollider.enabled = true;
        if (playerMovement) playerMovement.SetHiding(false);

        hidingManager.SetHidingObject(null);

        if (!exitWithFilter)
        {
            AudioSnapshotController.Instance.StopSnapshot();
        }

        if (camController != null)
        {
            if (exitToFirstPerson)
            {
                camController.SetFirstPerson();
            }
            else
            {
                camController.SetThirdPerson();
            }
        }

        isInside = false;
    }

    Vector3 GetBestExitDirection()
    {
        if (legalExitDirections.Length == 0) return transform.right;

        Vector3 cameraViewDir = playerCamera.transform.forward;
        Vector3 bestDirection = Vector3.zero;
        float maxDot = -1f;

        foreach (ExitDirection dir in legalExitDirections)
        {
            Vector3 worldDirection = DirectionHelper.GetWorldDirection(dir);
            float dotProduct = Vector3.Dot(cameraViewDir, worldDirection);
            if (dotProduct > maxDot)
            {
                maxDot = dotProduct;
                bestDirection = worldDirection;
            }
        }

        return bestDirection;
    }

    public Transform GetExitPoint()
    {
        return exitPoint;
    }

    public void SetIsInside(bool boolean)
    {
        isInside = boolean;
    }

    public void SetExitToFirstPerson(bool value)
    {
        exitToFirstPerson = value;
    }

    public bool GetExitToFirstPerson()
    {
        return exitToFirstPerson;
    }

    public void SetExitWithFilter(bool value)
    {
        exitWithFilter = value;
    }

    public bool GetExitWithFilter()
    {
        return exitWithFilter;
    }
}
