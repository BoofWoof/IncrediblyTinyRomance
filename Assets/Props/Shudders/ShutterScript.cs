using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterScript : MonoBehaviour
{
    public static ShutterScript instance;

    public List<GameObject> Shutters;
    public float RaisedHeight = 2.638f;
    public float LoweredHeight = 0;

    public float LiftDuration = 4f;
    public float DropDuration = 0.3f;

    private bool IsCoroutineRunning = false;

    public static bool ShuttersLowered = true;

    public AudioSource ShudderAudioSource;
    public AudioClip ShudderRaiseClip;
    public AudioClip SudderDropClip;

    public delegate void ShutterStateCallback(bool raised);
    static public event ShutterStateCallback ShutterToggled;

    private bool FirstRaise = true;

    public AudioSource Siren;

    public bool ForceShutdown = false;

    public GameObject PhoneToDelete;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        InstantClose();
        //InstantOpen();
    }

    public void DestroyPhone()
    {
        if (PhoneToDelete != null) Destroy(PhoneToDelete);
    }

    public void InstantClose()
    {
        ShuttersLowered = true;
        foreach (GameObject shutter in Shutters)
        {
            Animator animator = shutter.GetComponent<Animator>();
            animator.SetBool("Open", false);
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 1f);
            animator.Update(0f); // Force immediate update
        }
        CrossfadeScript.SetLowpassOn(true);
        MusicSelectorScript.SetOverworldSong(5, true);
        PPManagerScript.instance.ImmediateEmergencyPPFilter(false);
    }
    public void InstantOpen()
    {
        ShuttersLowered = false;
        foreach (GameObject shutter in Shutters)
        {
            Animator animator = shutter.GetComponent<Animator>();
            animator.SetBool("Open", true);
            animator.Play("Open", 0, 1f);
            animator.Update(0f); // Force immediate update
        }
        CrossfadeScript.SetLowpassOn(false);
        MusicSelectorScript.SetOverworldSong(1, true);
        PPManagerScript.instance.ImmediateEmergencyPPFilter(true);
    }

    public void ForceShutterLockdownToggle()
    {
        ForceShutdown = !ForceShutdown;
        if (!ShuttersLowered && ForceShutdown)
        {
            Debug.Log("Triggered");
            ActivateShutters();
        }
        if(ShuttersLowered && !ForceShutdown)
        {
            ActivateShutters();
        }
    }


    public void ActivateShutters()
    {
        if (FirstRaise)
        {
            FirstRaise = false;
            HudScript.SetContinueTutorial();
            PhonePositionScript.AllowPhoneToggle = true;
            DestroyPhone();
        }

        if (ShudderAudioSource.isPlaying) return;

        Debug.Log("Shutters Raising");
        if (ShuttersLowered)
        {
            if (ForceShutdown) return;
            if (RaiseShutters())
            {
                CrossfadeScript.SetLowpassOn(false);
                MusicSelectorScript.RevertOverworldSong();

                Siren.Pause();
                ShuttersLowered = false;
            } 
        }
        else
        {
            if (LowerShutters())
            {
                CrossfadeScript.SetLowpassOn(true);
                MusicSelectorScript.SetOverworldSong(5);


                Siren.Play();
                ShuttersLowered = true;
            } 
        }
    }
    private bool RaiseShutters()
    {
        if (!IsCoroutineRunning)
        {
            ShutterOpen();
            ShudderAudioSource.clip = ShudderRaiseClip;
            ShudderAudioSource.Play();
            ShutterToggled.Invoke(true);
            return true;
        }
        return false;
    }

    private bool LowerShutters()
    {
        if (!IsCoroutineRunning)
        {
            if(DayInfo.DayEndEnabled) ActiveBroadcast.BroadcastActivation("ShowEndDayScreen");
            ShutterClose();
            ShudderAudioSource.clip = SudderDropClip;
            ShudderAudioSource.Play();
            ShutterToggled.Invoke(false);
            return true;
        }
        return false;
    }

    public void ShutterClose()
    {

        foreach (GameObject shutter in Shutters)
        {
            Animator animator = shutter.GetComponent<Animator>();
            animator.SetBool("Open", false);
        };
    }

    public void ShutterOpen()
    {

        foreach (GameObject shutter in Shutters)
        {
            Animator animator = shutter.GetComponent<Animator>();
            animator.SetBool("Open", true);
        };
    }
}
