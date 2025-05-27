using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private bool isTurning;
    private float lastSpeed;

    [SerializeField] float walkTurnThreshold = 0.1f;
    [SerializeField] float turnLeftAngle = 60f;
    [SerializeField] float turnRightAngle = -60f;
    [SerializeField] float turn180Angle = 135f;
    [SerializeField] float turn90Duration = 0.6f;
    [SerializeField] float turn180Duration = 1.2f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        animator.applyRootMotion = isTurning;

        lastSpeed = speed;

        if (agent.pathPending || !agent.hasPath || isTurning)
            return;

        Vector3 desired = agent.desiredVelocity.normalized;
        if (desired.sqrMagnitude < 0.01f) return;

        float angle = Vector3.SignedAngle(transform.forward, desired, Vector3.up);
        float absAngle = Mathf.Abs(angle);

        if (speed < walkTurnThreshold)
        {
            if (absAngle >= turn180Angle)
            {
                if (angle > 0)
                    animator.SetTrigger("Turn180L_Idle");
                else
                    animator.SetTrigger("Turn180R_Idle");

                PauseAgent(turn180Duration);
                isTurning = true;
            }
            else if (angle > turnLeftAngle)
            {
                animator.SetTrigger("Turn90L_Idle");
                PauseAgent(turn90Duration);
                isTurning = true;
            }
            else if (angle < turnRightAngle)
            {
                animator.SetTrigger("Turn90R_Idle");
                PauseAgent(turn90Duration);
                isTurning = true;
            }
        }
    }

    public void OnTurnComplete()
    {
        isTurning = false;
        animator.applyRootMotion = false;
    }

    public void PauseAgent(float duration)
    {
        StartCoroutine(PauseAgentCoroutine(duration));
    }

    IEnumerator PauseAgentCoroutine(float duration)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(duration);
        agent.isStopped = false;
    }
    public void Footstep()
    {
        RuntimeManager.PlayOneShot("event:/GuardFootstep", transform.position);
    }
}
