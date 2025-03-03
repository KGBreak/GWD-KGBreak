using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;

[System.Serializable]
public class PathPoint
{
    public Transform position;
    public float timeSpent;
    public Actions action;
    public Vector3 direction;
}
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] List<PathPoint> pathPoints;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] float investigateTime;
    [SerializeField] Actions investigateAction;
    Coroutine currentActionCoroutine;
    float arrivalThreshold = 0.2f;
    float timer = 0;
    int nextPoint = 0;
    bool followingPath = true;

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
        // Set goal pos
        if (nextPoint >= pathPoints.Count)
        {
            nextPoint = 0;
        }
        navAgent.destination = pathPoints[nextPoint].position.position;

        // If goal pos reached
        if (HasReachedDestination())
        {
            navAgent.updateRotation = false;
            // We make our own direction if none is given.
            if (pathPoints[nextPoint].direction == Vector3.zero)
            {
                try
                {
                    pathPoints[nextPoint].direction = pathPoints[nextPoint].position.position - pathPoints[nextPoint-1].position.position;
                }
                catch (ArgumentOutOfRangeException)
                {
                    pathPoints[nextPoint].direction = pathPoints[nextPoint].position.position - pathPoints[pathPoints.Count - 1].position.position;
                }
            }

            rotateGoal(pathPoints[nextPoint].direction);

            // We have been at goal pos for correct amount of time, stop current action and update goal point
            if (timer > pathPoints[nextPoint].timeSpent)
            {
                StopAction();
                nextPoint = nextPoint + 1;
                timer = 0;
            }
            // We perfrom action and count time
            else
            {
                if (currentActionCoroutine == null && pathPoints[nextPoint].action != null)
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
            navAgent.updateRotation = false;
            if (timer > investigateTime)
            {
                StopAction();
                timer = 0;
                followingPath = true;
            }
            else
            {
                if (currentActionCoroutine == null)
                {
                    currentActionCoroutine = StartCoroutine(investigateAction.PerformAction());
                }
                timer += Time.deltaTime;
            }
        }
    }
    bool HasReachedDestination()
    {
        if (navAgent.pathPending || navAgent.remainingDistance > arrivalThreshold ) return false;
        return true;
    }

    void StopAction()
    {
        if (currentActionCoroutine != null)
        {
            pathPoints[nextPoint].action.InterruptAction();
            StopCoroutine(currentActionCoroutine);
            currentActionCoroutine = null;
        }
        navAgent.updateRotation = true;
    }

    void rotateGoal(Vector3 goalRotation)
    {
        // Turn in direction
        if (timer == 0.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(goalRotation, Vector3.up);
            transform.rotation = Quaternion.LookRotation(goalRotation, Vector3.up);
        }
    }

    public void SetDestination(Vector3 goalPos)
    {
        StopAction();
        followingPath = false;
        timer = 0;
        navAgent.destination = goalPos;
    }
}
