using UnityEngine;
using UnityEngine.AI;

public class InvestigateState : BaseState
{
    private float timer = 0f;
    private Coroutine currentActionCoroutine;
    private TurnOn btn;
    public InvestigateState(Enemy enemy) : base(enemy) { }

    public override void EnterState()
    {
        timer = 0f;
        currentActionCoroutine = null;
        btn = _enemy.GetTurnOnButton();

        _agent.destination = _enemy.GetInvestigateTarget().Value;
    }

    public override void ExitState()
    {
        if (currentActionCoroutine != null)
        {
            _enemy.StopCoroutine(currentActionCoroutine);
        }
        if (btn != null)
        {
            btn.TurnOnOrOff(false);
            _enemy.SetTurnOnButton(null);
        }
        _enemy.SetInvestigateTarget(null);
        _agent.updateRotation = true;
    }

    public override void Execute()
    {

        if (_enemy.GetInvestigateTarget().HasValue &&
    Vector3.Distance(_agent.destination, _enemy.GetInvestigateTarget().Value) > 0.3f)
        {
            if (currentActionCoroutine != null)
            {
                _enemy.StopCoroutine(currentActionCoroutine);
                currentActionCoroutine = null;
            }
            _agent.updateRotation = true;
            _agent.destination = _enemy.GetInvestigateTarget().Value;
            timer = 0f;
        }


        if (HasReachedDestination())
        {
            _agent.updateRotation = false;

            if (timer > _enemy.GetInvestigateTime())
            {
                // will exit soon
            }
            else
            {
                if (currentActionCoroutine == null)
                {
                    currentActionCoroutine = _enemy.StartCoroutine(_enemy.GetInvestigateAction().PerformAction());
                }
                timer += Time.deltaTime;
            }
        }
    }

    public override BaseState GetNextState()
    {
        if (_enemy.GetDebug())
        {
            Debug.Log(timer > _enemy.GetInvestigateTime());
        }
        return timer > _enemy.GetInvestigateTime() ? new PatrolState(_enemy) : this;
    }

    private bool HasReachedDestination()
    {
        Vector3 current = _enemy.transform.position;
        Vector3 target = _agent.destination;
        current.y = 0;
        target.y = 0;
        return Vector3.Distance(current, target) <= 0.2f;
    }

}
