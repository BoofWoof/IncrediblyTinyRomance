using UnityEngine;

public class ForceNearPlane : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Camera>().nearClipPlane = 0.001f;
    }
}
