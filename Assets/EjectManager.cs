using UnityEngine;
using UnityEngine.InputSystem;

public class EjectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private HidingManager hidingManager;
    private ItemManager itemManager;
    private void Start()
    {
        hidingManager = gameObject.GetComponent<HidingManager>();
        itemManager = gameObject.GetComponent<ItemManager>();
    }

    public void OnEject(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (hidingManager.GetIsHiding())
            {
                hidingManager.ExitHidingObject();
            }
            else
            {
                itemManager.EjectCurrentItem();
            }
        }
    }
}
