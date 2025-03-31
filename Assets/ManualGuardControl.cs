using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ManualGuardControl : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 move = direction * moveSpeed;
            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
    }
}
