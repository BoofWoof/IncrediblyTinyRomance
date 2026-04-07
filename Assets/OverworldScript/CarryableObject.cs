using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CarryableObject : MonoBehaviour
{
    public ReleasePointNode CurrentReleaseNode;

    public int ObjectID = -1;
    public string ObjectName;

    public static Dictionary<string, CarryableObject> CarryableObjects = new Dictionary<string, CarryableObject>();

    public UnityEvent ObjectActivate;

    public bool City;

    public Color LightColor;
    public void Start()
    {
        AddToCarryableObjects(false);

        if (City) return;
        if (CurrentReleaseNode != null) CurrentReleaseNode.heldObject = this;

        ObjectID = PropManager.nameToID[ObjectName];
    }

    public void AddToCarryableObjects(bool replaceExisting)
    {
        string lowerCaseName = ObjectName.ToLower();

        if (replaceExisting)
        {
            if (CarryableObjects.ContainsKey(lowerCaseName))
            {
                if(CarryableObjects[lowerCaseName] != this && CarryableObjects[lowerCaseName] != null) Destroy(CarryableObjects[lowerCaseName].gameObject);
                CarryableObjects.Remove(lowerCaseName);
            }
        } else
        {
            if (CarryableObjects.ContainsKey(lowerCaseName)) return;
        }
        CarryableObjects.Add(lowerCaseName, this);
    }

    public void Activate()
    {
        Debug.Log($"Activating Bounce: {name}");
        ObjectActivate?.Invoke();
    }

    public static CarryableObject GetCarryableObject(string objectName)
    {
        if (!CarryableObjects.ContainsKey(objectName.ToLower())) return null;
        return CarryableObjects[objectName.ToLower()];
    }

    public void OnEnable()
    {
        if (!CarryableObjects.ContainsKey(ObjectName.ToLower())) AddToCarryableObjects(false);
    }
    public void OnDisable()
    {
        if(CarryableObjects.ContainsValue(this)) CarryableObjects.Remove(ObjectName.ToLower());

        Release();
    }

    public void Release()
    {
        if (City) return;
        if (CurrentReleaseNode == null) return;
        if (CurrentReleaseNode.heldObject != this) return;
        CurrentReleaseNode.heldObject = null;
        CurrentReleaseNode = null;
    }

    public void GoTo(int NodeIdx)
    {
        Transform EndTransform = ObjectNodeTracker.Instance.CityNodes[NodeIdx].transform;

        transform.parent = EndTransform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (City) return;
        CurrentReleaseNode = ObjectNodeTracker.Instance.CityNodes[NodeIdx];
        ObjectNodeTracker.Instance.CityNodes[NodeIdx].heldObject = this;
    }

}
