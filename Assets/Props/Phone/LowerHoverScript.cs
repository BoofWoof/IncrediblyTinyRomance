using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LowerHoverScript : MonoBehaviour, IPointerEnterHandler
{
    public PhonePositionScript phone_control;
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(phone_control.LowerPhone());
    }

}
