using UnityEngine;

public class SkyCamFlat : MonoBehaviour
{
    public GameObject SkyCamera;

    public float yRotOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        Quaternion currentRotation = SkyCamera.transform.parent.rotation;
        Quaternion playerRoation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * yRotOffset);


        SkyCamera.transform.rotation = currentRotation * playerRoation;
    }
}
