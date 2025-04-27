using UnityEngine;
using UnityEngine.InputSystem;

public class HidingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private HideIn hidingObject;


    public void SetHidingObject(HideIn newHidingObject)
    {
        hidingObject = newHidingObject;
    }


    public HideIn GetHidingObject()
    {
        return hidingObject;
    }

    public void MoveHidingObject()
    {
        var exitPoint = hidingObject?.GetExitPoint()?.gameObject.GetComponent<HideIn>();

        if (hidingObject != null && exitPoint != null)
        {
            hidingObject.ExitObject();
            exitPoint.EnterObject();
        }

    }
    public bool GetIsHiding()
    {
        if (hidingObject == null)
        {
            return false;
        }
        else {
            return true;
        }
    }
}
