using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonFloatSampler
{
    public static float SamplePoisson(float lambda)
    {
        if (lambda <= 0f)
        {
            Debug.LogError("Lambda must be positive.");
            return 0f;
        }

        float L = Mathf.Exp(-lambda); // Threshold
        float k = 0f;
        float p = 1f;

        do
        {
            k += 1f; // Increment by 1
            p *= Random.value; // Generate uniform random [0,1)
        } while (p > L);

        // Subtract a random fraction to make the output continuous
        return k - 1f + Random.value;
    }
}
