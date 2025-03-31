using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float elevationThreshold = 0.3f;

    Interactable closestInteractable = null;

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        float closestDistance = Mathf.Infinity;
        closestInteractable = null; // Reset before checking

        foreach (Collider hit in hits)
        {
            float heightDifference = Mathf.Abs(transform.position.y - hit.transform.position.y);
            if (heightDifference <= elevationThreshold) {
                if (hit.gameObject.layer == LayerMask.NameToLayer("Selectable"))
                {
                    hit.gameObject.layer = LayerMask.NameToLayer("Interactable");
                }

                Interactable interactable = hit.GetComponent<Interactable>();
                if (interactable == null) continue; // Skip if not interactable

                interactable.ResetTimer();

                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }

            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && closestInteractable != null)
        {
            closestInteractable.InteractWith();
        }
    }
}

