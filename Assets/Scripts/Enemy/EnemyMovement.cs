using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Controls;
using Util;

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
    TurnOn turnOnButton;

    // Update is called once per frame
    void Update()
    {

        /*if (followingPath) {
            FollowPath();
        }
        else
        {
            Investigate();
        }*/
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
                if (turnOnButton != null)
                {
                    turnOnButton.TurnOnOrOff(false);
                    turnOnButton = null;
                }
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

    void Investigate(TurnOn turnOnButton)
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

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = navAgent.destination;

        // Ignore Y axis
        currentPosition.y = 0;
        targetPosition.y = 0;

        float distance = Vector3.Distance(currentPosition, targetPosition);
        return distance <= arrivalThreshold;
    }

    void StopAction()
    {
        if (currentActionCoroutine != null)
        {
            if (pathPoints[nextPoint].action != null)
            {
                pathPoints[nextPoint].action.InterruptAction();
            }
            StopCoroutine(currentActionCoroutine);
            currentActionCoroutine = null;
        }
        navAgent.updateRotation = true;
    }

    void rotateGoal(Vector3 goalRotation)
    {
        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(goalRotation, Vector3.up);

        // Gradually rotate towards the target over timeSpent duration
        float lerpFactor = Mathf.Clamp01(timer / pathPoints[nextPoint].timeSpent);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpFactor);
    }

    public void SetDestination(Vector3 goalPos)
    {
        StopAction();
        followingPath = false;
        timer = 0;
        navAgent.destination = goalPos;
    }

    public void SetDestination(Vector3 goalPos, TurnOn turnOnButton)
    {
        StopAction();
        followingPath = false;
        timer = 0;
        navAgent.destination = goalPos;
        this.turnOnButton = turnOnButton;
    }
}
