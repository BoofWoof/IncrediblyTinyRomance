using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PPManagerScript : MonoBehaviour
{
    [Header("Emergency")]
    public Volume EmergencyPP;
    public float EmergencyFadePeriod = 2f;

    [Header("Phone")]
    public Volume PhonePP;
    public float PhoneFadePeriod = 2f;

    [Header("Standard")]
    public Volume StandardPP;
    public float StandardFadePeriod = 2f;

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
        if (raised)
        {
            StartCoroutine(AdjustPPFilter(EmergencyPP, 0, EmergencyFadePeriod));
        } else
        {
            StartCoroutine(AdjustPPFilter(EmergencyPP, 1, EmergencyFadePeriod/2f));
        }
    }
    private void AdjustPhonePPFilter(bool raised)
    {
        if (raised)
        {
            StartCoroutine(AdjustPPFilter(PhonePP, 1, EmergencyFadePeriod));
            StartCoroutine(AdjustPPFilter(StandardPP, 0, EmergencyFadePeriod));
        }
        else
        {
            StartCoroutine(AdjustPPFilter(PhonePP, 0, EmergencyFadePeriod/2f));
            StartCoroutine(AdjustPPFilter(StandardPP, 1, EmergencyFadePeriod/2f));
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
