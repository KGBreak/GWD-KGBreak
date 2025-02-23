using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class SimulateDetection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] float investigateTime;
    [SerializeField] Vector3 investPos;
    float timer = 0;
    bool done = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > investigateTime && done == false) {
            enemyMovement.SetDestination(investPos);
            done = true;
        }
    }
}
