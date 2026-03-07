using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public float rotationSpeed = 720f;
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.Self);
    }
}
