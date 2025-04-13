using UnityEngine;
using System.Collections;
using FMODUnity;
//using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    private GameObject m_Item;
    private Collider playerCollider;


    private void Start()
    {
        playerCollider = GetComponent<Collider>();
    }

    public void SetItem(GameObject newItem)
    {
        EjectCurrentItem(); // Remove current item if any

        m_Item = newItem;

        // Make the item a child of the player
        newItem.transform.SetParent(transform);
        newItem.transform.localPosition = Vector3.zero; // Place it inside the player

        // Optionally, reset its rotation
        newItem.transform.localRotation = Quaternion.identity;

        // Disable physics so it doesn't fall/move on its own
        Rigidbody itemRb = newItem.GetComponent<Rigidbody>();
        Collider itemCl = newItem.GetComponent<Collider>();
        if (itemRb != null && itemCl != null)
        {
            itemRb.isKinematic = true;
            itemRb.useGravity = false;
            itemCl.enabled = false;
        }

    }

    public void EjectCurrentItem()
    {
        if (m_Item == null) return;
        RuntimeManager.PlayOneShot("event:/Player/Drop");

        // Remove it from being a child of the player
        m_Item.transform.SetParent(null);

        // Move the item slightly in front of the player
        m_Item.transform.position = transform.position - (transform.forward*0.5f);

        // Activate physics again
        Rigidbody itemRb = m_Item.GetComponent<Rigidbody>();
        Collider itemCl = m_Item.GetComponent<Collider>();
        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            itemRb.useGravity = true;

            // Apply a slight push outward
            Vector3 ejectForce = (-transform.forward + Vector3.up * 0.5f);
            itemRb.AddForce(ejectForce, ForceMode.Impulse);
        }

        // Re-enable collision after a delay
        Physics.IgnoreCollision(playerCollider, itemCl, true);
        itemCl.enabled = true;
        StartCoroutine(EnableCollisionAfterDelay(itemCl, 0.2f));

        // Clear reference to the item
        m_Item = null;
    }

    private IEnumerator EnableCollisionAfterDelay(Collider itemCl, float delay)
    {
        yield return new WaitForSeconds(delay);

        Physics.IgnoreCollision(playerCollider, itemCl, false);
    }

    public GameObject GetItem()
    {
        return m_Item;
    }
}


