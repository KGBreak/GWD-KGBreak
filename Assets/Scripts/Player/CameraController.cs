using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    float mouseSensitivity = 100f;
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
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
    }

    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);

        Vector3 desiredPosition = playerTransform.position - rotation * Vector3.forward * distanceFromPlayer + offset;
        transform.position = desiredPosition;

        transform.LookAt(playerTransform.position + offset);
    }
}