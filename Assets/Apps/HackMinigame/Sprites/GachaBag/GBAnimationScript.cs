using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBAnimationScript : MonoBehaviour
{
    public GachaBagScript ManagerScript;

    public void SummonItem()
    {
        ManagerScript.SummonItem();
    }
}
