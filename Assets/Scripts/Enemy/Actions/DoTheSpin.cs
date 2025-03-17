using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DoTheSpin : Actions
{
    public override IEnumerator PerformAction()
    {
        isInterrupted = false;
        while (!isInterrupted)
        {

            transform.Rotate(0.0f, 500.0f * Time.deltaTime, 0.0f);
            yield return null;
        }
    }
}
