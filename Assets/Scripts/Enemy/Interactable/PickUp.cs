using UnityEngine;
using FMODUnity;

public class PickUp : Interactable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ItemManager playerItemManager;


    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Get all Renderers from child objects
        playerItemManager = player.GetComponent<ItemManager>();
    }
    public override void InteractWith()
    {
        playerItemManager.SetItem(this.gameObject);
        RuntimeManager.PlayOneShot("event:/Player/Pickup");
    }
}
