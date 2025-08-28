using UnityEngine;

public class CityScript : CarryableObject
{
    public static CityScript Instance;

    public int CurrentNode;

    public void OnEnable()
    {
        Instance = this;
    }

    public void GoTo(int NodeIdx)
    {
        Transform EndTransform = ObjectNodeTracker.Instance.CityNodes[NodeIdx].transform;

        transform.position = EndTransform.position;
        transform.rotation = EndTransform.rotation;
    }
}
