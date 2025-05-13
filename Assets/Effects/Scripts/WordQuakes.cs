using System;
using UnityEngine;
using UnityEngine.Audio;

public class WordQuakes : MonoBehaviour
{
    public AudioSource rumbleSource;

    public int sampleSize = 1024; // Number of samples to get
    private float[] samples;

    public GameObject City;
    public float DistanceFalloff = 1f;

    public GameObject Head;

    void Start()
    {
        samples = new float[sampleSize];
        transform.parent = Head.transform;
        transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float reduction = 1* DistanceFalloff / Vector3.Distance(City.transform.position, transform.position);
        reduction = Mathf.Clamp(reduction, 0.1f, 1);

        rumbleSource.GetOutputData(samples, 0); // 0 = left channel

        float sum = 0f;
        for (int i = 0; i < sampleSize; i++)
        {
            sum += samples[i] * samples[i]; // square them to get energy
        }

        float rmsValue = Mathf.Sqrt(sum / sampleSize); // root mean square (RMS)
        float amplitude = rmsValue; // RMS is a good approximation of "loudness"


        MoveCamera.moveCamera.SetRumble(amplitude * reduction);
    }
}
