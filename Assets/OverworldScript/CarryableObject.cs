using System.Collections.Generic;
using UnityEngine;

public class CarryableObject : MonoBehaviour
{
    public string ObjectName;

    public static List<CarryableObject> CarryableObjects = new List<CarryableObject>();


    public void Start()
    {
        CarryableObjects.Add(this);
    }

    public static CarryableObject GetCarryableObject(string objectName)
    {
        foreach (CarryableObject obj in CarryableObjects)
        {
            if (obj.ObjectName.ToLower() == objectName.ToLower())
            {
                return obj;
            }
        }
        return null;
    }

    public void OnDestroy()
    {
        CarryableObjects.Remove(this);
    }

    public void GoTo(int NodeIdx)
    {
        Transform EndTransform = ObjectNodeTracker.Instance.CityNodes[NodeIdx].transform;

        transform.position = EndTransform.position;
        transform.rotation = EndTransform.rotation;
        transform.parent = EndTransform;
    }

}
