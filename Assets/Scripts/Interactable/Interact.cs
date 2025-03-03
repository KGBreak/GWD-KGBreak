using UnityEngine;

public class Interact : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float detectionRadius;
    [SerializeField] LayerMask targetLayer; // Assign in Inspector

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        Interactable closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            interactable.LightUp();

            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }

        }
    }
}
