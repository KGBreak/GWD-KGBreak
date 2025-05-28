using FMODUnity;
using System.Net;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class EndingSequence : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] VoiceLine firstVoiceLine;
    [SerializeField] VoiceLine secondVoiceLine;
    [SerializeField] Transform issac;
    [SerializeField] GameObject player;
    [SerializeField] GameObject finalKeyCard;

    private ItemManager itemManager;
    private bool firstEnter = true;

    private void Start()
    {
        itemManager = player.GetComponent<ItemManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (firstEnter && other.CompareTag("Player"))
        {
            firstEnter = false;
            RuntimeManager.PlayOneShotAttached(firstVoiceLine.eventRef, issac.gameObject);
        }
        else if (itemManager.GetItem() == finalKeyCard)
        {
            RuntimeManager.PlayOneShotAttached(secondVoiceLine.eventRef, issac.gameObject);
        }
 


    }
}
