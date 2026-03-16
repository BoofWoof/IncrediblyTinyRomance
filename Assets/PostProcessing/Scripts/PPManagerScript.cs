using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PPManagerScript : MonoBehaviour
{
    public static PPManagerScript instance;

    [Header("Emergency")]
    public Volume EmergencyPP;
    public float EmergencyFadePeriod = 2f;
    private Coroutine EmergencyPPCoroutine;

    [Header("Phone")]
    public Volume PhonePP;
    public float PhoneFadePeriod = 2f;
    private Coroutine PhonePPCoroutine;

    [Header("Standard")]
    public Volume StandardPP;
    public float StandardFadePeriod = 2f;
    private Coroutine StandardPPCoroutine;

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EmergencyPP.weight = 1;
        ShutterScript.ShutterToggled += AdjustEmergencyPPFilter;
        PhonePositionScript.PhoneToggled += AdjustPhonePPFilter;
    }

    private void OnDisable()
    {
        ShutterScript.ShutterToggled -= AdjustEmergencyPPFilter;
        PhonePositionScript.PhoneToggled -= AdjustPhonePPFilter;

    }

    private void AdjustEmergencyPPFilter(bool raised)
    {
        if(EmergencyPPCoroutine != null) StopCoroutine(EmergencyPPCoroutine);
        if (raised)
        {
            EmergencyPPCoroutine = StartCoroutine(AdjustPPFilter(EmergencyPP, 0, EmergencyFadePeriod));
        } else
        {
            EmergencyPPCoroutine = StartCoroutine(AdjustPPFilter(EmergencyPP, 1, EmergencyFadePeriod/2f));
        }
    }

    public void ImmediateEmergencyPPFilter(bool raised)
    {
        if (EmergencyPPCoroutine != null) StopCoroutine(EmergencyPPCoroutine);
        if (raised)
        {
            EmergencyPP.weight = 0f;
        }
        else
        {
            EmergencyPP.weight = 1f;
        }
    }
    private void AdjustPhonePPFilter(bool raised)
    {
        if (PhonePPCoroutine != null) StopCoroutine(PhonePPCoroutine);
        if (StandardPPCoroutine != null) StopCoroutine(StandardPPCoroutine);

        if (raised)
        {
            PhonePPCoroutine = StartCoroutine(AdjustPPFilter(PhonePP, 1, EmergencyFadePeriod));
            StandardPPCoroutine = StartCoroutine(AdjustPPFilter(StandardPP, 0, EmergencyFadePeriod));
        }
        else
        {
            PhonePPCoroutine = StartCoroutine(AdjustPPFilter(PhonePP, 0, EmergencyFadePeriod/2f));
            StandardPPCoroutine = StartCoroutine(AdjustPPFilter(StandardPP, 1, EmergencyFadePeriod/2f));
        }
    }

    IEnumerator AdjustPPFilter(Volume targetVolume, float finalWeight, float updateDuration)
    {

        float timeElapsed = 0f;
        float startingWeight = targetVolume.weight;

        // Gradually crossfade over the fadeDuration
        while (timeElapsed < updateDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / updateDuration;
            targetVolume.weight = Mathf.Lerp(startingWeight, finalWeight, progress);

            yield return null;
        }

        targetVolume.weight = finalWeight;
        yield return null;
    }
}
