using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HackingShoe", menuName = "Hacking/Equipment/Shoe", order = 2)]
public class HackingShoeSO : HackingEquipmentSO
{
    public float AttackSpeedLambda = 0.5f;
    public float MinAttackSpeed = 0.1f;

    public float AttackSpeed = 0f;

    override public void RandomizeValues()
    {
        AttackSpeed = PoissonFloatSampler.SamplePoisson(AttackSpeedLambda);
        AttackSpeed = Mathf.Max(AttackSpeed, MinAttackSpeed);
        Debug.Log(AttackSpeed);
    }
}
