using UnityEngine;

public class TurnOn : Interactable
{

    [SerializeField] GameObject suckUp;
    SuckUp suckUpScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        suckUpScript = suckUp.GetComponent<SuckUp>();
    }
    public override void InteractWith()
    {
        if (suckUpScript.IsTurnedOn())
        {
            suckUpScript.SetTurnOn(false);
        }
        else
        {
            suckUpScript.SetTurnOn(true);
        }
        SetRotation();
    }

    public void SetRotation()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -transform.eulerAngles.z);
    }
}
