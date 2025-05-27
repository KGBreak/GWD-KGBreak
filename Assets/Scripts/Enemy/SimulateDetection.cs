using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class SimulateDetection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Enemy enemy;
    [SerializeField] float investigateTime;
    [SerializeField] float investigateTime2;
    [SerializeField] Vector3 investPos;
    [SerializeField] Vector3 investPos2;
    float timer = 0;
    bool done = false;
    bool done2 = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > investigateTime && done == false) {
            enemy.SetInvestigateTarget(investPos);
            done = true;
        }
        if (timer > investigateTime2 && done2 == false)
        {
            enemy.SetInvestigateTarget(investPos2);
            done2 = true;
        }
    }
}
