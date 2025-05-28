using UnityEngine;
using FMODUnity;

public class TurnOn : Interactable
{

    [SerializeField] GameObject suckUp;
    [SerializeField] GameObject vacuumSoundObject;
    [SerializeField] float pingRange = 10f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask obstacleLayer;
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
        FindAndPingClosest();
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

    public void FindAndPingClosest()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, pingRange, enemyLayer);
        Transform closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;

        Debug.Log(enemies.Length);

        foreach (var enemy in enemies)
        {
            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            float distanceSqr = directionToEnemy.sqrMagnitude;

            // Check line of sight
            if (!Physics.Raycast(transform.position, directionToEnemy.normalized, directionToEnemy.magnitude, obstacleLayer))
            {
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestEnemy = enemy.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            if (closestEnemy.TryGetComponent<Enemy>(out Enemy enemyScript))
            {
                enemyScript.SetTurnOnButton(this);
                enemyScript.SetInvestigateTarget(transform.position);
            }
        }
    }
}
