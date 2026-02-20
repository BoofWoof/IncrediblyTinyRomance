using UnityEngine;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    public static GameControls PlayerInputs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        PlayerInputs = new GameControls();
        PlayerInputs.Disable();
        PlayerInputs.Overworld.Disable();
        PlayerInputs.Phone.Disable();

        PhonePositionScript.PhoneToggled += PhoneToggle;
    }

    public static void GameStart()
    {
        PlayerInputs.Enable();
        PlayerInputs.Overworld.Enable();
        PlayerInputs.Phone.Disable();
    }
    public static void GameEnd()
    {
        AllOff();
    }
    public static void AllOn()
    {
        PlayerInputs.Enable();
        PlayerInputs.Overworld.Enable();
        PlayerInputs.Phone.Enable();
    }
    public static void AllOff()
    {
        PlayerInputs.Disable();
        PlayerInputs.Overworld.Disable();
        PlayerInputs.Phone.Disable();
    }

    private void OnDisable()
    {
        PhonePositionScript.PhoneToggled -= PhoneToggle;
        PlayerInputs.Overworld.Disable();
        PlayerInputs.Phone.Disable();
        PlayerInputs.Disable();
    }

    private void OnEnable()
    {
        PhonePositionScript.PhoneToggled += PhoneToggle;
        PlayerInputs.Enable();
        PlayerInputs.Overworld.Enable();
        PlayerInputs.Phone.Enable();
    }

    private void PhoneToggle(bool raised)
    {
        if (raised)
        {
            PlayerInputs.Overworld.Disable();
            PlayerInputs.Phone.Enable();
        }
        else
        {
            PlayerInputs.Overworld.Enable();
            PlayerInputs.Phone.Disable();
        }
    }
}
