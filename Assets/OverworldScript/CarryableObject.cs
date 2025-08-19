using UnityEngine;

public class CarryableObject : MonoBehaviour
{
    public void GoTo(int NodeIdx)
    {
        Transform EndTransform = ObjectNodeTracker.Instance.CityNodes[NodeIdx].transform;

        transform.position = EndTransform.position;
        transform.rotation = EndTransform.rotation;
        transform.parent = EndTransform;
    }

}
