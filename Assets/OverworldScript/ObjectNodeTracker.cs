using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ObjectNodeTracker : MonoBehaviour
{
    public List<CityNode> CityNodes = new List<CityNode>();

    public static ObjectNodeTracker Instance;

    public void OnEnable()
    {
        Instance = this;
    }
}
