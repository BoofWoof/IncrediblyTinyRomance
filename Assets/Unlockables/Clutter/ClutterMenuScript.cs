using PixelCrushers;
using UnityEngine;

public class ClutterMenuScript : MonoBehaviour
{
    public bool WasPhoneUnLocked = false;

    public void OnEnable()
    {
        CursorStateControl.AllowMouse(true);
        PlayerCam.EnableCameraMovement = false;
        WasPhoneUnLocked = PhonePositionScript.AllowPhoneToggle;
        PhonePositionScript.LockPhoneDown();
    }

    public void OnDisable()
    {
        CursorStateControl.AllowMouse(false);
        PlayerCam.EnableCameraMovement = true;
        if(WasPhoneUnLocked) PhonePositionScript.UnlockPhone();
    }
}
