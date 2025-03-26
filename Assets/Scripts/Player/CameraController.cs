using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    Settings settings; // Reference to the Settings ScriptableObject
    [SerializeField]
    float distanceFromPlayer = 3f;
    [SerializeField]
    Vector3 offset = new Vector3(0, 1.5f, 0);

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
        float sensitivityMultiplier = 20.0f; // Increase this value to make the sensitivity faster
        float mouseX = lookInput.x * settings.mouseSensetivity * sensitivityMultiplier * Time.deltaTime;
        float mouseY = lookInput.y * settings.mouseSensetivity * sensitivityMultiplier * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
    }

    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);

        Vector3 desiredPosition = playerTransform.position - rotation * Vector3.forward * distanceFromPlayer + offset;
        Vector3 directionToPlayer = playerTransform.position + offset - desiredPosition;

        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position + offset, -directionToPlayer.normalized, out hit, distanceFromPlayer))
        {
            desiredPosition = playerTransform.position + offset - directionToPlayer.normalized * hit.distance;
        }

        transform.position = desiredPosition;
        transform.LookAt(playerTransform.position + offset);
    }
}