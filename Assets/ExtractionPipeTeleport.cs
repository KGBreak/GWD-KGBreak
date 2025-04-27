using UnityEngine;

public class ExtractionPipeTeleport : MonoBehaviour
{
    [SerializeField] VentPoint ventPoint;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ventPoint.InteractWith();
        }
    }
}
