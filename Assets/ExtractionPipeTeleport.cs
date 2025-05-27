using UnityEngine;

public class ExtractionPipeTeleport : MonoBehaviour
{
    [SerializeField] VentPointEDIT ventPoint;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ventPoint.InteractWith();
        }
    }
}
