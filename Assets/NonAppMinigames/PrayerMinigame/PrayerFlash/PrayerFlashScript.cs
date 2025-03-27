using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerFlashScript : MonoBehaviour
{
    public float FlashLengthSec = 0.5f;
    public float MinShift = 0f;
    public float MaxShift = 10f;
    public AnimationCurve amplitudeCurve;

    private Renderer renderer;
    private MaterialPropertyBlock block;


    public void Start()
    {
        renderer = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();

        renderer.GetPropertyBlock(block);
        block.SetFloat("_Shift", MinShift); // Example: Change color
        renderer.SetPropertyBlock(block);
    }

    public void ActivateFlash()
    {
        StartCoroutine(FlashOnce());
    }

    IEnumerator FlashOnce()
    {
        float timePassedSec = 0f;

        Debug.Log("Flash Start");

        while (timePassedSec < FlashLengthSec)
        {
            timePassedSec += Time.deltaTime;

            float shift = (MaxShift-MinShift) * amplitudeCurve.Evaluate(timePassedSec / FlashLengthSec) + MinShift;

            renderer.GetPropertyBlock(block);
            block.SetFloat("_Shift", shift); // Example: Change color
            renderer.SetPropertyBlock(block);
            yield return null;
        }
        Debug.Log("Flash End");
    }
}
