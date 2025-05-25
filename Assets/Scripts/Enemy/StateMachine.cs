using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class StateMachine: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected BaseState _currentState;
    protected bool _isTransitioning;
    protected Enemy _enemy;


    void Start()
    {
        _enemy = GetComponent<Enemy>();
        _isTransitioning = false;
        _currentState = new PatrolState(_enemy);
        _currentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {

        BaseState nextState = _currentState.GetNextState();
        if (_currentState.Equals(nextState) && !_isTransitioning)
        {
            _currentState.Execute();
        } else if (!_isTransitioning)
        {
            ChangeState(nextState);
        }

    }

    void ChangeState(BaseState nextState)
    {
        _isTransitioning = true;
        _currentState.ExitState();
        _currentState = nextState;
        _currentState.EnterState();
        _isTransitioning = false;
    }
}
