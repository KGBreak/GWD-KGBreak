//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.Rendering;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Interact : MonoBehaviour
{
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float elevationThreshold = 0.3f;
    [SerializeField] GameObject feeler;
    [SerializeField] HidingManager hidingManager;
    [SerializeField] ItemManager itemManager;

    Interactable closestInteractable = null;

    void Update()
    {

        if (hidingManager.GetIsHiding())
        {
            closestInteractable = hidingManager.GetHidingObject().gameObject.GetComponent<Interactable>();
        }
        else {

            Collider[] hits = Physics.OverlapSphere(feeler.transform.position, detectionRadius, targetLayer);

            float closestDistance = Mathf.Infinity;
            closestInteractable = null; // Reset before checking

            foreach (Collider hit in hits)
            {
                float heightDifference = Mathf.Abs(feeler.transform.position.y - hit.transform.position.y);
                if (heightDifference <= elevationThreshold || hit.transform.gameObject.CompareTag("Door"))
                {
                    Interactable interactable = hit.GetComponent<Interactable>();
                    if (interactable == null) continue; // Skip if not interactable

                    float distance = Vector3.Distance(feeler.transform.position, hit.transform.position);

                    if (distance < closestDistance && itemManager.GetItem() != interactable.gameObject)
                    {
                        closestDistance = distance;
                        closestInteractable = interactable;
                    }

                }
            }

        }

        if (closestInteractable != null)
        {
            if (closestInteractable.gameObject.layer == LayerMask.NameToLayer("Selectable"))
            {
                closestInteractable.gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
            closestInteractable.ResetTimer();
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

