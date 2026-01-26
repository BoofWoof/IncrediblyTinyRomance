using UnityEngine;

public class ChannelChanger : MonoBehaviour
{
    public GameObject AerialDefense;
    public GameObject PurityDefense;
    public GameObject LockDefense;

    public static ChannelChanger ActiveChannelChanger;
    public static bool DangerActive = false;

    public static ChannelChanger instance;

    public void Start()
    {
        instance = this;

        ActiveChannelChanger = this;
        LockSwitch();
    }

    public void LockSwitch()
    {
        if (DangerActive) return;
        PhonePositionScript.UnlockPhone();

        AerialDefense.SetActive(false);
        PurityDefense.SetActive(false);
        LockDefense.SetActive(true);
    }

    public void AerialSwitch()
    {
        if (DangerActive) return;
        PhonePositionScript.LockPhoneDown();

        AerialDefense.SetActive(true);
        PurityDefense.SetActive(false);
        LockDefense.SetActive(false);
    }

    public void PuritySwitch()
    {
        if (DangerActive) return;
        PhonePositionScript.LockPhoneDown();

        AerialDefense.SetActive(false);
        PurityDefense.SetActive(true);
        LockDefense.SetActive(false);
    }
}
