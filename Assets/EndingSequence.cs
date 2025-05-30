using FMODUnity;
using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;

public class EndingSequence : VoiceRoom
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Dialog firstVoiceLine;
    [SerializeField] Dialog secondVoiceLine;
    [SerializeField] Dialog lockdownIntercom;
    [SerializeField] Transform issac;
    [SerializeField] GameObject player;
    [SerializeField] GameObject finalKeyCard;

    public GameObject allLights;
    private Animator endingSequenceAnimator;
    public Animator fadeToBlackAnimator;

    private ItemManager itemManager;
    private bool firstEnter = true;
    private bool gameOver = false;

    private void Start()
    {
        endingSequenceAnimator = GetComponent<Animator>();
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
        else if (itemManager.GetItem() == finalKeyCard && !gameOver)
        {
            voiceManager.OnPlayerEnterRoom(this, secondVoiceLine);
            gameOver = true;
            StartCoroutine(EndSequence());
            //RuntimeManager.PlayOneShotAttached(secondVoiceLine.eventRef, issac.gameObject);
        }
    }

    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(17f);
        allLights.SetActive(false);
        endingSequenceAnimator.SetTrigger("EndingSequence");
        fadeToBlackAnimator.Play("Ending Sequence Screen");
        yield return new WaitForSeconds(7f);
        voiceManager.OnPlayerEnterRoom(this, lockdownIntercom);
        yield return new WaitForSeconds(25f);
        SceneManager.LoadScene("StartMenu");
    }
}
