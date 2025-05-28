using UnityEngine;

public class OpeningConsumeLore : ConsumeLore
{
    [SerializeField] private GameObject openingSequence;

    private openingSequence OS;
    void Start()
    {
        OS = openingSequence.GetComponent<openingSequence>();
    }

    public override void InteractWith()
    {
        OS.consumeDictionary();
        base.InteractWith();

    }
}
