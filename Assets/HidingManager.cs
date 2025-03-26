using UnityEngine;

public class HidingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private HideIn hidingObject;


    public void SetHidingObject(HideIn newHidingObject)
    {
        hidingObject = newHidingObject;
    }



    public void ExitHidingObject()
    {
        if (hidingObject != null) {
            hidingObject.ExitObject();
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
