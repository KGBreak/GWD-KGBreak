using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState
{
    protected Enemy _enemy;
    protected NavMeshAgent _agent;

    protected BaseState(Enemy enemy)
    {
        _enemy = enemy;
        _agent = enemy.GetNavMeshAgent();
    }
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void Execute();
    public abstract BaseState GetNextState();
}
