using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class ButtonSFX : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public EventReference clickSoundEvent;
    public EventReference hoverSoundEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(clickSoundEvent);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(hoverSoundEvent);
    }
}
