using UnityEngine;
using System.Collections;

public abstract class Actions : MonoBehaviour
{
    [SerializeField] protected EnemyMovement EnemyMovement;
    [SerializeField] protected Transform EnemyTransform;
    protected bool isInterrupted = false;
    public abstract IEnumerator PerformAction();

    // Call this to interrupt an action
    public void InterruptAction()
    {
        isInterrupted = true;
    }
}
