using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class DoTheBoogey : Actions
{
   public override IEnumerator PerformAction()
    {
        isInterrupted = false;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();


        while (!isInterrupted)
        {
            agent.enabled = false;
            transform.Rotate(0.0f, 0.0f, 500.0f * Time.deltaTime);
            yield return null;
        }
    }
}
