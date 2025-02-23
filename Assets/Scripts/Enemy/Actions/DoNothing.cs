using System.Collections;
using UnityEngine;

public class DoNothing : Actions
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override IEnumerator PerformAction()
    {
        isInterrupted = false;
        while (!isInterrupted) {
            Debug.Log("Doing nothing");
            yield return null;
        }
    }
}
