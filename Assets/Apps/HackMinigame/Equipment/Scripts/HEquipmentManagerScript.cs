using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HEquipmentManagerScript : MonoBehaviour
{
    public static HEquipmentManagerScript EquipmentManager;

    [Header("EquipmentSlots")]
    public HackingEquipmentSlotScript AttackEquipment;
    public HackingEquipmentSlotScript DefenseEquipment;
    public HackingEquipmentSlotScript SpeedEquipment;
    public HackingEquipmentSlotScript DamageModifier;
    public HackingEquipmentSlotScript DefenseModifier;
    public HackingEquipmentSlotScript OnHitModifier;

    private void Awake()
    {
        EquipmentManager = this;
    }
    public void AssignNewEquipment(HackingEquipmentSO newEquipment)
    {
        if (newEquipment is HackingWeaponSO)
        {
            AttackEquipment.SetEquipment(newEquipment);
            CombatantScript.PlayerCombatant.BaseAttack = ((HackingWeaponSO)newEquipment).Attack;
        }
        if (newEquipment is HackingHelmetSO)
        {
            DefenseEquipment.SetEquipment(newEquipment);
            CombatantScript.PlayerCombatant.BaseDefense = ((HackingHelmetSO)newEquipment).Defense;
        }
        if (newEquipment is HackingShoeSO)
        {
            SpeedEquipment.SetEquipment(newEquipment);
            CombatantScript.PlayerCombatant.BaseAttackRate = ((HackingShoeSO)newEquipment).AttackSpeed;
        }
    }
}
