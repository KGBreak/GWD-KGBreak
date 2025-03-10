using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private GameObject m_Item;

    public void SetItem(GameObject newItem)
    {
        EjectCurrentItem();

        m_Item = newItem;

        newItem.SetActive(false);

    }

    public void EjectCurrentItem()
    {
        if (m_Item == null) return;

        m_Item.SetActive(true);

        m_Item.transform.position = transform.position + transform.forward;

        // Get the item's Rigidbody
        Rigidbody itemRb = m_Item.GetComponent<Rigidbody>();

        if (itemRb != null)
        {
            itemRb.linearVelocity = Vector3.zero; // Reset velocity to prevent weird physics issues

            // Apply a slight push upward and forward
            Vector3 ejectForce = (transform.forward + Vector3.up * 0.5f) * 2f;
            itemRb.AddForce(ejectForce, ForceMode.Impulse);
        }

        // Clear reference to the item
        m_Item = null;
    }

    public GameObject GetItem() { 
        return m_Item;
    }
}
