using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Outline outliner;
    float counter = 0;
    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;


        if (counter > 1)
        {
            LightDown();
        }
    }

    public void LightUp()
    {
        outliner.enabled = true;
        counter = 0;
    }

    public void LightDown()
    {
        outliner.enabled = false;
    }

    public virtual void InteractWith() {
        Debug.Log("WOW I HAVE BEEN INTERACTED WITH. I AM: " + name);
    }
}
