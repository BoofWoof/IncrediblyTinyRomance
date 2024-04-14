using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaiseHoverScript : MonoBehaviour, IPointerEnterHandler
{
    public PhonePositionScript phone_control;
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(phone_control.RaisePhone());
    }

}
