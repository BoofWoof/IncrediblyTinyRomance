using PixelCrushers;
using UnityEngine;

public class ClutterMenuScript : MonoBehaviour
{
    public void OnEnable()
    {
        CursorStateControl.AllowMouse(true);
        PlayerCam.EnableCameraMovement = false;
    }

    public void OnDisable()
    {
        CursorStateControl.AllowMouse(false);
        PlayerCam.EnableCameraMovement = true;
    }
}
