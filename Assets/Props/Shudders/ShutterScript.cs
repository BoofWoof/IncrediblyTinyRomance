using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterScript : MonoBehaviour
{
    public List<GameObject> Shutters;
    public float RaisedHeight = 2.638f;
    public float LoweredHeight = 0;

    public float LiftDuration = 4f;
    public float DropDuration = 0.3f;

    private bool IsCoroutineRunning = false;
    private float CurrentHeight = 0;

    public bool ShuttersLowered = true;

    public AudioSource ShudderAudioSource;
    public AudioClip ShudderRaiseClip;
    public AudioClip SudderDropClip;

    public delegate void ShutterStateCallback(bool raised);
    static public event ShutterStateCallback ShutterToggled;

    private bool FirstRaise = true;

    public AudioSource Siren;

    public static ShutterScript instance;

    public bool ForceShutdown = false;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        foreach (GameObject shutter in Shutters)
        {
            Animator animator = shutter.GetComponent<Animator>();
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 1f);
            animator.Update(0f); // Force immediate update
        };
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
                PhonePositionScript.AllowPhoneToggle = true;
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
                PhonePositionScript.AllowPhoneToggle = false;
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
