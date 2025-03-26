using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement pm;
    private bool isJelly = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {

        if (pm.MoveInput.sqrMagnitude > 0.01f)
        {
            animator.SetBool("IsMoving", true);;
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        if (Input.GetKey(KeyCode.J))
        {
            if (!isJelly)
            {
                animator.SetTrigger("TurnJelly");
                isJelly = true;
            }
            if (isJelly)
            {
                animator.SetTrigger("TurnGloop");
                isJelly = false;
            }
        }
    }
}
