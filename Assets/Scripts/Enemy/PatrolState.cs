using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState
{
    private float timer;
    private Coroutine currentActionCoroutine;

    public PatrolState(Enemy enemy) : base(enemy) { }

    public override void EnterState()
    {
        // Reset timer and coroutine when entering patrol state
        timer = 0f;
        currentActionCoroutine = null;
    }

    public override void ExitState()
    {
        // Interrupt any ongoing action when exiting state
        if (currentActionCoroutine != null && _enemy.GetPathPoints()[_enemy.GetPathIndex()].action != null)
        {
            _enemy.GetPathPoints()[_enemy.GetPathIndex()].action.InterruptAction();
        }
        _agent.updateRotation = true;
    }

    public override void Execute()
    {

        var pathPoints = _enemy.GetPathPoints();
        int index = _enemy.GetPathIndex();
        if (index >= pathPoints.Count) _enemy.SetPathIndex(0);

        var point = pathPoints[_enemy.GetPathIndex()];
        _agent.destination = point.position.position;

        if (HasReachedDestination())
        {
            _agent.updateRotation = false;

            // Determine direction for rotating the agent
            Vector3 dir = point.direction == Vector3.zero && _enemy.GetPathIndex() > 0 ?
                          point.position.position - pathPoints[_enemy.GetPathIndex() - 1].position.position :
                          point.direction;

            RotateTowards(dir);

            // Wait at the destination for the defined time before moving on
            if (timer > point.timeSpent)
            {
                if (currentActionCoroutine != null)
                    _enemy.StopCoroutine(currentActionCoroutine);

                _enemy.SetPathIndex((_enemy.GetPathIndex() + 1) % pathPoints.Count);
                timer = 0f;
            }
            else
            {
                // Start the action if one exists
                if (currentActionCoroutine == null && point.action != null)
                {
                    currentActionCoroutine = _enemy.StartCoroutine(point.action.PerformAction());
                }
                timer += Time.deltaTime;
            }
        }
    }

    public override BaseState GetNextState()
    {
        // If an investigation target is set, switch state
        return _enemy.GetInvestigateTarget().HasValue ? new InvestigateState(_enemy) : this;
    }

    private bool HasReachedDestination()
    {
        Vector3 current = _enemy.transform.position;
        Vector3 target = _agent.destination;
        current.y = 0; target.y = 0;
        return Vector3.Distance(current, target) <= 0.2f;
    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRot = Quaternion.LookRotation(direction);
        float lerp = Mathf.Clamp01(timer / _enemy.GetPathPoints()[_enemy.GetPathIndex()].timeSpent);
        _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRot, lerp);
    }
}