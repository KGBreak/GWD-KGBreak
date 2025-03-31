using System.Linq;
using UnityEngine;

public class SuckUp : MonoBehaviour
{
    private bool isTurnedOn = true;
    [SerializeField] float pullForce = 3;
    private float lastTriggerTime = 0f;
    private float triggerGracePeriod = 0.1f; // Time before we consider it "no longer staying"
    private PlayerMovement playerMovement;
    private bool pullingPlayer = false;


    private void Start()
    {
        playerMovement = GameObject.FindGameObjectsWithTag("Player").First().GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        if (Time.time - lastTriggerTime > triggerGracePeriod)
        {
            OnTriggerNoLongerStay();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isTurnedOn && other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>().SetGravity(pullForce);
            pullingPlayer = true;
            lastTriggerTime = Time.time; // Reset timer
        }
    }

    private void OnTriggerNoLongerStay()
    {
        if (pullingPlayer)
        {
            playerMovement.resetGravity();
            pullingPlayer = false;
        }
    }
    public void SetTurnOn(bool boolean)
    {
        isTurnedOn = boolean;
    }
    public bool IsTurnedOn()
    {
        return isTurnedOn;
    }
}
