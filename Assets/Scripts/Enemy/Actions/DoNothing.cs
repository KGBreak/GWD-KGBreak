using System.Collections;
using UnityEngine;

public class DoNothing : Actions
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override IEnumerator PerformAction()
    {
        isInterrupted = false;
        while (!isInterrupted) {
            yield return null;
        }
    }
}
