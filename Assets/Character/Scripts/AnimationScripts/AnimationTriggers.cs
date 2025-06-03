using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    public GameObject LeftHandGrabber;
    public void Impact(float Strength)
    {
        MoveCamera.moveCamera.ShakeScreen(1f, Strength);
    }

    public void GrabLeft()
    {
        GameObject Grabbable = LeftHandGrabber.GetComponent<CollissionTracker>().GetFirstTarget();
        Grabbable.transform.parent = LeftHandGrabber.transform;
        Grabbable.transform.localPosition = Vector3.zero;
        Grabbable.transform.localRotation = Quaternion.identity;
    }
}
