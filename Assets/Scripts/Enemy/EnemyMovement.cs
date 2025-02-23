using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

[System.Serializable]
public class PathPoint
{
    public Vector3 position;
    public float timeSpent;
    public Actions action;
}
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] List<PathPoint> pathPoints;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] float investigateTime;
    Coroutine currentActionCoroutine;
    float arrivalThreshold = 0.2f;
    float timer = 0;
    int lastPos;
    int nextPoint = 0;
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
        if (nextPoint >= pathPoints.Count)
        {
            nextPoint = 0;
        }
        navAgent.destination = pathPoints[nextPoint].position;

        if (HasReachedDestination())
        {
            if (timer > pathPoints[nextPoint].timeSpent)
            {
                if (currentActionCoroutine != null)
                {
                    pathPoints[nextPoint].action.InterruptAction();
                    StopCoroutine(currentActionCoroutine);
                    currentActionCoroutine = null;
                    navAgent.updateRotation = true;
                }

                nextPoint = nextPoint + 1;
                timer = 0;
            }
            else
            {
                if (currentActionCoroutine == null)
                {
                    currentActionCoroutine = StartCoroutine(pathPoints[nextPoint].action.PerformAction());
                }
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
        if (navAgent.pathPending || navAgent.remainingDistance > arrivalThreshold ) return false;
        return true;
    }

    public void SetDestination(Vector3 goal)
    {
        timer = 0;
        navAgent.destination = goal;
        followingPath = false;
    }
}
