using FMODUnity;
using System.Net;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class EndingSequence : VoiceRoom
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Dialog firstVoiceLine;
    [SerializeField] Dialog secondVoiceLine;
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
            Debug.Log(this);
            voiceManager.OnPlayerEnterRoom(this, firstVoiceLine);
            //RuntimeManager.PlayOneShotAttached(firstVoiceLine.eventRef, issac.gameObject);
        }
        else if (itemManager.GetItem() == finalKeyCard)
        {
            voiceManager.OnPlayerEnterRoom(this, secondVoiceLine);
            //RuntimeManager.PlayOneShotAttached(secondVoiceLine.eventRef, issac.gameObject);
        }
 


    }
}
