using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarryableObject : MonoBehaviour
{
    public ReleasePointNode CurrentReleaseNode;

    public int ObjectID = -1;
    public string ObjectName;

    public static List<CarryableObject> CarryableObjects = new List<CarryableObject>();

    public UnityEvent ObjectActivate;

    public bool City;

    public Color LightColor;
    public void Start()
    {
        if(!CarryableObjects.Contains(this)) CarryableObjects.Add(this);

        if (City) return;
        if (CurrentReleaseNode != null) CurrentReleaseNode.heldObject = this;

        ObjectID = PropManager.nameToID[ObjectName];
    }

    public void Activate()
    {
        Debug.Log($"Activating Bounce: {name}");
        ObjectActivate?.Invoke();
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

    public void OnEnable()
    {
        if (!CarryableObjects.Contains(this)) CarryableObjects.Add(this);
    }
    public void OnDisable()
    {
        CarryableObjects.Remove(this);

        Release();
    }

    public void OnDestroy()
    {
    }

    public void Release()
    {
        if (City) return;
        if (CurrentReleaseNode == null) return;
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
