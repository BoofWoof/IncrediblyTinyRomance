using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CityNodeTracker : MonoBehaviour
{
    public List<CityNode> CityNodes = new List<CityNode>();

    public static CityNodeTracker Instance;

    public void OnEnable()
    {
        Instance = this;
    }
}
