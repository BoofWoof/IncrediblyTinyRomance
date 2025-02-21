using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    public AnimationCurve amplitudeCurve;
    private Vector3 cameraShakeOffset;
    public ParticleSystem dustGenerator;

    public static MoveCamera moveCamera;
    public static float rumble;

    public AudioSource earthquakeSoundSource;
    public AudioSource rumbleSoundSource;

    private void Start()
    {
        moveCamera = this;
    }
    void Update()
    {
        transform.position = cameraPosition.position + cameraShakeOffset + Random.insideUnitSphere * rumble;
    }

    public void TestShake(float durationSec)
    {
        ShakeScreen(durationSec, 1);
    }
    public void SetRumble(float rumbleQuantity)
    {
        rumble = rumbleQuantity * 0.2f;
        rumbleSoundSource.volume = rumbleQuantity / 0.2f;
        rumbleSoundSource.pitch = 1 + rumbleQuantity * 2f;
    }
    public void ShakeScreen(float durationSec, float shakeAmplitude = 1)
    {
        moveCamera.StartCoroutine(moveCamera.AddScreenShake(durationSec, shakeAmplitude));
    }

    public IEnumerator AddScreenShake(float durationSec, float shakeAmplitude = 1)
    {
        dustGenerator.Play();
        earthquakeSoundSource.Play();

        float timePassedSec = 0;
        while (timePassedSec < durationSec)
        {
            yield return null;
            timePassedSec += Time.deltaTime;
            cameraShakeOffset = Random.insideUnitSphere * shakeAmplitude * amplitudeCurve.Evaluate(timePassedSec/durationSec);
        }

        cameraShakeOffset = Vector3.zero;

        StartCoroutine(AudioFadeOut.FadeOut(earthquakeSoundSource, 2.5f));
    }
}
