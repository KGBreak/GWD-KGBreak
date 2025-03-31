using UnityEngine;

public class SuckUp : MonoBehaviour
{
    private bool isTurnedOn = true;
    [SerializeField] float pullForce = 1f;


    void OnTriggerEnter(Collider other)
    {
        if (isTurnedOn)
        {
            if (other.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerMovement>().SetGravity(3f);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //other.gameObject.GetComponent<PlayerMovement>().SetGravity(-9.81f);
        }

    }
}
