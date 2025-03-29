using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "HackingWeapon", menuName = "Hacking/Equipment/Weapon", order = 2)]
public class HackingWeaponSO : HackingEquipmentSO
{
    public float AttackLambda = 0.5f;
    public float MinAttack = 0.1f;

    public float Attack = 0f;

    override public void RandomizeValues()
    {
        Attack = PoissonFloatSampler.SamplePoisson(AttackLambda);
        Attack = Mathf.Max(Attack, MinAttack);
        Debug.Log(Attack);
    }
}
