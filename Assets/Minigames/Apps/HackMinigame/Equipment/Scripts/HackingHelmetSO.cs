using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HackingHelmet", menuName = "Hacking/Equipment/Helmet", order = 2)]
public class HackingHelmetSO : HackingEquipmentSO
{
    public float DefenseLambda = 50f;
    public float MinDefense = 1f;

    public float Defense = 0f;

    override public void RandomizeValues()
    {
        Defense = PoissonFloatSampler.SamplePoisson(DefenseLambda);
        Defense = Mathf.Max(Defense, MinDefense);
        Debug.Log(Defense);
    }
}
