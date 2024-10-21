using UnityEngine;

public class TrukAppScript : AppScript
{
    public static Canvas PhoneScreenCanvas;
    public Canvas phoneScreenCanvas;

    private void Awake()
    {
        PhoneScreenCanvas = phoneScreenCanvas;
        Hide(true);
        RegisterInputActions();
    }
}
