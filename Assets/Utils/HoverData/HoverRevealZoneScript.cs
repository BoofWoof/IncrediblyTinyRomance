using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverRevealZoneScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnHover;
    public UnityEvent OnUnHover;


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUnHover?.Invoke();
    }
}
