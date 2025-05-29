using UnityEngine;
using FMODUnity;

public class TurnOn : Interactable
{

    [SerializeField] GameObject suckUp;
    [SerializeField] GameObject vacuumSoundObject;
    [SerializeField] Enemy enemy1;
    SuckUp suckUpScript;
    private VacuumSFX vacuumSFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        suckUpScript = suckUp.GetComponent<SuckUp>();
        if (vacuumSoundObject != null)
            vacuumSFX = vacuumSoundObject.GetComponent<VacuumSFX>();
    }
    public override void InteractWith()
    {
        if (suckUpScript.IsTurnedOn())
        {
            suckUpScript.SetTurnOn(false);
            RuntimeManager.PlayOneShot("event:/TurnOff", transform.position);
            if (vacuumSFX != null)
                vacuumSFX.StopVacuumSound();
        }
        else
        {

            suckUpScript.SetTurnOn(true);
            RuntimeManager.PlayOneShot("event:/TurnOff", transform.position);
            if (vacuumSFX != null)
                vacuumSFX.PlayVacuumSound();
        }
        SetRotation();
        PingClosest();
    }

    public void TurnOnOrOff(bool turnOn)
    {
        suckUpScript.SetTurnOn(turnOn);
        SetRotation();
    }

    public void SetRotation()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -transform.eulerAngles.z);
    }

    public void PingClosest()
    {
        if (enemy1 != null)
        {
            enemy1.SetTurnOnButton(this);
            enemy1.SetInvestigateTarget(transform.position);
        }
        
    }
}
