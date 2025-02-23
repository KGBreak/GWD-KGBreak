using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Vector3[] positions;
    [SerializeField] float[] timeSpent;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] float investigateTime;
    float arrivalThreshold = 0.2f;
    float timer = 0;
    int lastPos;
    int nextPos = 0;
    bool followingPath = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (followingPath) {
            FollowPath();
        }
        else
        {
            Investigate();
        }
    }

    void FollowPath()
    {
        if (nextPos >= positions.Length)
        {
            nextPos = 0;
        }
        navAgent.destination = positions[nextPos];

        if (HasReachedDestination())
        {
            if (timer > timeSpent[nextPos])
            {
                nextPos = nextPos + 1;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    void Investigate()
    {
        if (HasReachedDestination())
        {
            if (timer > investigateTime)
            {
                followingPath = true;
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }
    bool HasReachedDestination()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance > arrivalThreshold ) return false;
        return true;
    }

    public void SetDestination(Vector3 goal)
    {
        timer = 0;
        navAgent.destination = goal;
        followingPath = false;
    }

    /*public void SetNewPath()
    {
        
    }*/
}
