using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class IntroSoundEffects : MonoBehaviour
{
    public AudioClip Gate;
    public AudioClip Step;

    private AudioSource ThisAudioSource;

    public UnityEvent OnCompleteEvent;


    public void Start()
    {
        ThisAudioSource = GetComponent<AudioSource>();
    }

    public void TakeStep()
    {
        ThisAudioSource.clip = Step;
        ThisAudioSource.Play();
    }
    public void OpenGate()
    {
        ThisAudioSource.clip = Gate;
        ThisAudioSource.Play();
    }

    public void OnCutsceneCompletion()
    {
        OnCompleteEvent?.Invoke();
        Destroy(gameObject);
    }
}
