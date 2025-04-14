using UnityEngine;
using UnityEngine.UIElements;

public class FollowObject : MonoBehaviour
{
    public Transform targetTransform;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
