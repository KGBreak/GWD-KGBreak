using UnityEngine;
using System.Collections;
//using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    private GameObject m_Item;

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
        if (itemRb != null)
        {
            itemRb.isKinematic = true;
            itemRb.useGravity = false;
        }

    }

    public void EjectCurrentItem()
    {
        if (m_Item == null) return;

        // Remove it from being a child of the player
        m_Item.transform.SetParent(null);

        // Move the item slightly in front of the player
        m_Item.transform.position = transform.position + (transform.forward*0.2f);

        // Activate physics again
        Rigidbody itemRb = m_Item.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            itemRb.useGravity = true;

            // Apply a slight push outward
            Vector3 ejectForce = (transform.forward*2f + Vector3.up*2f);
            itemRb.AddForce(ejectForce, ForceMode.Impulse);
        }
        // Clear reference to the item
        m_Item = null;


    }


    public GameObject GetItem()
    {
        return m_Item;
    }
}


