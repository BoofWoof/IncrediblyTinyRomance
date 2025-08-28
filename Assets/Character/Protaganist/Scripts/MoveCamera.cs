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
    public static float rumble;
    public static float shake;

    public static float targetRumbleQuantity;
    public float currentRumbleQuantity = 0f;
    public float falloffRate = 1f;

    public AudioSource earthquakeSoundSource;
    public AudioClip[] earthquakeSoundOptions;
    public AudioSource rumbleSoundSource;

    private void Start()
    {
        moveCamera = this;
    }
    void Update()
    {
        transform.position = cameraPosition.position + cameraShakeOffset + Random.insideUnitSphere * rumble;


        if (targetRumbleQuantity > currentRumbleQuantity) currentRumbleQuantity = targetRumbleQuantity;
        currentRumbleQuantity = Mathf.MoveTowards(currentRumbleQuantity, targetRumbleQuantity, Time.deltaTime * falloffRate);

        rumble = currentRumbleQuantity * 0.2f;
        rumbleSoundSource.volume = currentRumbleQuantity * 10f;
        rumbleSoundSource.pitch = 1 + currentRumbleQuantity * 2f;
    }

    public void TestShake(float durationSec)
    {
        ShakeScreen(durationSec, 1);
    }
    public static void SetRumble(float rumbleQuantity)
    {
        targetRumbleQuantity = rumbleQuantity;
    }
    public void ShakeScreen(float durationSec, float shakeAmplitude = 1)
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
            shake = shakeAmplitude * amplitudeCurve.Evaluate(timePassedSec / durationSec);
            cameraShakeOffset = Random.insideUnitSphere * shake;
        }

        cameraShakeOffset = Vector3.zero;

        StartCoroutine(AudioFadeOut.FadeOut(earthquakeSoundSource, 2.5f));
    }
}
