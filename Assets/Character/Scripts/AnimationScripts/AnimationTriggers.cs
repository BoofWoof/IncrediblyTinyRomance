using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    public CollissionTracker LeftHandGrab;
    public CollissionTracker LeftHandRelease;
    public OverworldPositionScript overworldPositionScript;
    public void Impact(float Strength)
    {
        MoveCamera.moveCamera.ShakeScreen(1f, Strength);
    }

    public void GrabLeft()
    {
        GameObject Grabable = LeftHandGrab.GetFirstTarget();
        Grabable.transform.parent = LeftHandGrab.transform;
        Grabable.transform.localPosition = Vector3.zero;
        Grabable.transform.localRotation = Quaternion.identity;
    }

    public void DropLeft()
    {
        GameObject ReleasePoint = LeftHandRelease.GetFirstTarget();
        GameObject Grabable = LeftHandGrab.transform.GetChild(1).gameObject;
        Grabable.transform.parent = ReleasePoint.transform;
        Grabable.transform.localPosition = Vector3.zero;
        Grabable.transform.localRotation = Quaternion.identity;
    }

    public void EnableMobility()
    {
        overworldPositionScript.CharacterMobile = true;
    }
}
