using UnityEngine;

public class MaintainRotation : MonoBehaviour
{
    public bool x_lock = false;
    public bool y_lock = false;
    public bool z_lock = false;

    public float x_rot, y_rot, z_rot;

    public void Start()
    {
        x_rot = transform.rotation.eulerAngles.x;
        y_rot = transform.rotation.eulerAngles.y;
        z_rot = transform.rotation.eulerAngles.z;
    }

    public void Update()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        if (x_lock)
        {
            rotation.x = x_rot;
        }
        if (y_lock)
        {
            rotation.y = y_rot;
        }
        if (z_lock)
        {
            rotation.z = z_rot;
        }
        transform.rotation = Quaternion.Euler(rotation);
    }
}
