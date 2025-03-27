using UnityEngine;
using System.Collections;

public class Door : Interactable
{
    [SerializeField] float slideSpeed = 1f;
    [SerializeField] float autoCloseTime = 3f;
    [SerializeField] GameObject keyCard;
    [SerializeField] float slideLength = 1.8f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isMoving = false;
    private bool isOpen = false;
    private Coroutine movementCoroutine;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + (-transform.forward * slideLength);
    }

    public override void InteractWith()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        var item = player.GetComponent<ItemManager>().GetItem();

        if (item == keyCard && !isOpen)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(MoveDoor(openPosition, () =>
        {
            isOpen = true;
            movementCoroutine = StartCoroutine(AutoCloseAfterDelay());
        }));
    }

    void CloseDoor()
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(MoveDoor(closedPosition, () =>
        {
            isOpen = false;
        }));
    }

    IEnumerator MoveDoor(Vector3 targetPosition, System.Action onComplete)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
        onComplete?.Invoke();
    }

    IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(autoCloseTime);
        CloseDoor();
    }
}


