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

    public void ActivateShutters()
    {
        Debug.Log("Shutters Raising");
        if (ShuttersLowered)
        {
            if (RaiseShutters()) ShuttersLowered = false;
        }
        else
        {
            if (LowerShutters()) ShuttersLowered = true;
        }
    }
    private bool RaiseShutters()
    {
        if (!IsCoroutineRunning)
        {
            StartCoroutine(ShutterHeightChange(RaisedHeight, LiftDuration));
            ShudderAudioSource.clip = ShudderRaiseClip;
            ShudderAudioSource.Play();
            return true;
        }
        return false;
    }

    private bool LowerShutters()
    {
        if (!IsCoroutineRunning)
        {
            StartCoroutine(ShutterHeightChange(LoweredHeight, DropDuration));
            ShudderAudioSource.clip = SudderDropClip;
            ShudderAudioSource.Play();
            return true;
        }
        return false;
    }

    IEnumerator ShutterHeightChange(float newHeight, float updateDuration)
    {
        IsCoroutineRunning = true;

        float timeElapsed = 0f;
        float startingHeight = CurrentHeight;

        // Gradually crossfade over the fadeDuration
        while (timeElapsed < updateDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / updateDuration;
            CurrentHeight = Mathf.Lerp(startingHeight, newHeight, progress);

            foreach (GameObject shutter in Shutters)
            {
                Vector3 prevPos = shutter.transform.localPosition;
                shutter.transform.localPosition = new Vector3(prevPos.x, CurrentHeight, prevPos.z);
            }

            yield return null;
        }

        IsCoroutineRunning = false;
        yield return null;
    }
}
