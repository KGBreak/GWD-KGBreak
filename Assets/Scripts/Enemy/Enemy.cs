using System.Collections.Generic;
using UnityEngine;
using Util;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<PathPoint> pathPoints;
    [SerializeField] private float investigateTime;
    [SerializeField] private Actions investigateAction;
    [SerializeField] private bool debug = false;

    private int currentPathIndex = 0;
    private Vector3? investigateTarget = null;
    private TurnOn currentTurnOnButton = null;

    public NavMeshAgent GetNavMeshAgent() => agent;
    public List<PathPoint> GetPathPoints() => pathPoints;
    public int GetPathIndex() => currentPathIndex;

    public bool GetDebug() => debug;
    public void SetPathIndex(int index) => currentPathIndex = index;
    public float GetInvestigateTime() => investigateTime;
    public Actions GetInvestigateAction() => investigateAction;
    public Vector3? GetInvestigateTarget() => investigateTarget;
    public void SetInvestigateTarget(Vector3? target) => investigateTarget = target;
    public TurnOn GetTurnOnButton() => currentTurnOnButton;
    public void SetTurnOnButton(TurnOn btn) => currentTurnOnButton = btn;
}
