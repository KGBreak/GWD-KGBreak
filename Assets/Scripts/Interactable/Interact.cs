using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask targetLayer; // Assign in Inspector
    Interactable closestInteractable = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Selectable"))
            {
                hit.gameObject.layer = LayerMask.NameToLayer("Interactable");
            }

            Interactable interactable = hit.GetComponent<Interactable>();
            interactable.ResetTimer();

            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        if (hits.Length <1)
        {
            closestInteractable = null;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && closestInteractable != null) // Only trigger once per key press
        {
            closestInteractable.InteractWith();
        }
    }

}
