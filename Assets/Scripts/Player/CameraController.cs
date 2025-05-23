using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    Settings settings; // Reference to the Settings ScriptableObject
    public float distanceFromPlayer = 3f;

    [SerializeField]
    Vector3 offset = new Vector3(0, 1.5f, 0);
    [SerializeField]
    bool isFirstPerson = false; // Flag to toggle between first-person and third-person

    float xRotation = 0f;
    float yRotation = 0f;
    InputSystem_Actions playerInput;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Look.performed += OnLook;
        playerInput.Player.Look.canceled += OnLook;
    }

    private void OnDisable()
    {
        playerInput.Player.Look.performed -= OnLook;
        playerInput.Player.Look.canceled -= OnLook;
        playerInput.Player.Disable();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        float sensitivityMultiplier = 40.0f; // Increase this value to make the sensitivity faster
        float mouseX = lookInput.x * settings.mouseSensetivity * sensitivityMultiplier * Time.deltaTime;
        float mouseY = lookInput.y * settings.mouseSensetivity * sensitivityMultiplier * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
    }

    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);

        if (isFirstPerson)
        {
            // First-person view
            transform.position = playerTransform.position + offset;
            transform.rotation = rotation;
        }
        else
        {
            // Third-person view
            Vector3 desiredPosition = playerTransform.position - rotation * Vector3.forward * distanceFromPlayer + offset;
            Vector3 directionToPlayer = playerTransform.position + offset - desiredPosition;


            RaycastHit[] hits = Physics.RaycastAll(playerTransform.position + offset, -directionToPlayer.normalized, distanceFromPlayer);

            float closestDistance = distanceFromPlayer;
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Item") || hit.collider.CompareTag("Suck")) continue;

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                }
            }
            desiredPosition = playerTransform.position + offset - directionToPlayer.normalized * closestDistance;

            transform.position = desiredPosition;
            transform.LookAt(playerTransform.position + offset);
        }
    }
}