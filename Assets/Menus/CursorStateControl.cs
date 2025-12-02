using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class CursorStateControl : MonoBehaviour
{
    public static CursorStateControl ActiveCursorController;

    public static bool MenuUp;

    public static CursorLockMode LastLockMode = CursorLockMode.Locked;
    public static bool LastVisible = false;

    public void Awake()
    {
        ActiveCursorController = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MenuUp = false;

        PhonePositionScript.PhoneToggled += AllowMouse;
    }

    private void OnDestroy()
    {
        PhonePositionScript.PhoneToggled -= AllowMouse;
    }

    public static void PauseMenuToggle(bool menuUp)
    {
        MenuUp = menuUp;
        if (menuUp)
        {
            LastLockMode = Cursor.lockState;
            LastVisible = Cursor.visible;


            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = LastLockMode;
            Cursor.visible = LastVisible;
        }
    }

    public static void AllowMouse(bool allow)
    {
        if (allow)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
