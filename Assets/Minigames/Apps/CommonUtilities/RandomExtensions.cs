using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions
{
    public static Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    // Standard Gaussian (mean = 0, stdDev = 1)
    public static float Gaussian()
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        return Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
    }

    // Gaussian with custom mean and standard deviation
    public static float Gaussian(float mean, float stdDev)
    {
        return mean + stdDev * Gaussian();
    }
}
