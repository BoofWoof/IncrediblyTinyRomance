using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class CursorStateControl : MonoBehaviour
{
    public static bool MenuUp;
    public ToggleActive[] PauseMenus;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MenuUp = false;

        PhonePositionScript.PhoneToggled += AllowMouse;

        foreach(ToggleActive pauseMenu in PauseMenus) {
            pauseMenu.toggleActiveDelegate += PauseMenuToggle;
        }
    }

    private void OnDestroy()
    {
        PhonePositionScript.PhoneToggled -= AllowMouse;

        foreach (ToggleActive pauseMenu in PauseMenus)
        {
            pauseMenu.toggleActiveDelegate -= PauseMenuToggle;
        }
    }

    private void PauseMenuToggle(bool menuUp)
    {
        MenuUp = menuUp;
        if (menuUp)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            if (PhonePositionScript.raised)
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
