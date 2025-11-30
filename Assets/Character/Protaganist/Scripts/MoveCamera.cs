using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    public AnimationCurve amplitudeCurve;
    private Vector3 cameraShakeOffset;
    public ParticleSystem dustGenerator;

    public static MoveCamera moveCamera;
    private static float ImpactRumble;

    private static float TargetWordRumble { get; set; }
    private static float CurrentWordRumble { get; set; }

    private static float TargetMaintainRumble { get; set; }
    private static float CurrentMaintainRumble { get; set; }
    public static float TotalRumble { get; set; }

    public float falloffRate = 1f;

    public AudioSource earthquakeSoundSource;
    public AudioClip[] earthquakeSoundOptions;
    public AudioSource rumbleSoundSource;

    public static float VibrationIntensity = 1f;

    private void Start()
    {
        moveCamera = this;
    }
    void Update()
    {
        if (TargetWordRumble > CurrentWordRumble) CurrentWordRumble = TargetWordRumble;
        CurrentWordRumble = CurrentWordRumble * 1 / (1 + Time.deltaTime * 10f);

        if (TargetMaintainRumble >= CurrentMaintainRumble) CurrentMaintainRumble = TargetMaintainRumble;
        else CurrentMaintainRumble = Mathf.MoveTowards(CurrentMaintainRumble, TargetMaintainRumble, Time.deltaTime * falloffRate);

        TotalRumble = VibrationIntensity * (ImpactRumble + CurrentMaintainRumble + CurrentWordRumble);
        transform.position = cameraPosition.position + TotalRumble * Random.insideUnitSphere;

        float MaintainRumbles = CurrentMaintainRumble + CurrentWordRumble;
        rumbleSoundSource.volume = MaintainRumbles * 10f;
        rumbleSoundSource.pitch = 1 + MaintainRumbles * 2f;

        TargetWordRumble = 0;
    }

    public static void SetVibrationIntensity(float newIntensity)
    {
        VibrationIntensity = newIntensity;
    }

    public void TestShake(float durationSec)
    {
        ImpactShakeScreen(durationSec, 1);
    }
    public static void SetMaintainRumble(float rumbleQuantity)
    {
        TargetMaintainRumble = rumbleQuantity;
    }
    public static void SetWordRumble(float rumbleQuantity)
    {
        if (TargetWordRumble < rumbleQuantity)
        {
            TargetWordRumble = rumbleQuantity;
        }
    }
    public void ImpactShakeScreen(float durationSec, float shakeAmplitude = 1)
    {
        moveCamera.StartCoroutine(moveCamera.AddScreenShake(durationSec, shakeAmplitude));
    }

    public IEnumerator AddScreenShake(float durationSec, float shakeAmplitude = 1)
    {
        dustGenerator.Play();
        earthquakeSoundSource.volume = shakeAmplitude;
        AudioClip randomClip = earthquakeSoundOptions[Random.Range(0, earthquakeSoundOptions.Length)];

        earthquakeSoundSource.clip = randomClip;
        earthquakeSoundSource.Play();

        float timePassedSec = 0;
        while (timePassedSec < durationSec)
        {
            yield return null;
            timePassedSec += Time.deltaTime;
            ImpactRumble = shakeAmplitude * amplitudeCurve.Evaluate(timePassedSec / durationSec);
        }

        ImpactRumble = 0;

        StartCoroutine(AudioFadeOut.FadeOut(earthquakeSoundSource, 2.5f));
    }
}
