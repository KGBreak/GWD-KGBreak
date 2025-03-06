using UnityEngine;

public class Interactable : MonoBehaviour
{
    float counter = 0;
    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;


        if (counter > 0.1)
        {
            gameObject.layer = LayerMask.NameToLayer("Selectable");
        }
    }

    public void ResetTimer()
    {
        counter = 0;
    }

    public virtual void InteractWith() {
        Debug.Log("WOW I HAVE BEEN INTERACTED WITH. I AM: " + name);
    }
}
