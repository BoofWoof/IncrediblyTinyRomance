using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ObjectNodeTracker : MonoBehaviour
{
    public List<ReleasePointNode> CityNodes = new List<ReleasePointNode>();

    public static ObjectNodeTracker Instance;

    public void OnEnable()
    {
        Instance = this;
    }
}
