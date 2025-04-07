using UnityEngine;
using UnityEngine.InputSystem;

public class EjectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ItemManager itemManager;
    private void Start()
    {
        itemManager = gameObject.GetComponent<ItemManager>();
    }

    public void OnEject(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            itemManager.EjectCurrentItem();
        }
    }
}
