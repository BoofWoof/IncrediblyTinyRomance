using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackingEquipmentSlotScript : MonoBehaviour
{
    public Sprite DefaultSprite;
    public HackingEquipmentSO EquipmentData;

    public void Start()
    {
        if (EquipmentData == null)
        {
            SetSprite(DefaultSprite);
        } else
        {
            HEquipmentManagerScript.EquipmentManager.AssignNewEquipment(EquipmentData);
        }
    }

    public void SetEquipment(HackingEquipmentSO newEquipment)
    {
        EquipmentData = newEquipment;
        SetSprite(newEquipment.EquipmentSprite);
    }

    public void SetSprite(Sprite newSprite)
    {
        Image image = GetComponent<Image>();
        image.sprite = newSprite;
        image.SetNativeSize();
    }
}
