using UnityEngine;
using UnityEngine.Audio;

public class WordQuakes : MonoBehaviour
{
    public AudioSource rumbleSource;

    public int sampleSize = 1024; // Number of samples to get
    private float[] samples;

    void Start()
    {
        samples = new float[sampleSize];
    }

    // Update is called once per frame
    void Update()
    {
        rumbleSource.GetOutputData(samples, 0); // 0 = left channel

        float sum = 0f;
        for (int i = 0; i < sampleSize; i++)
        {
            sum += samples[i] * samples[i]; // square them to get energy
        }

        float rmsValue = Mathf.Sqrt(sum / sampleSize); // root mean square (RMS)
        float amplitude = rmsValue; // RMS is a good approximation of "loudness"

        Debug.Log(amplitude);

        MoveCamera.moveCamera.SetRumble(amplitude);
    }
}
