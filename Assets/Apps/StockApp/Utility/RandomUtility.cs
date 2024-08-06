using UnityEngine;
public static class RandomUtility
{
    public static float GenerateGaussian(float mean = 0, float standardDeviation = 1)
    {
        float u1 = Random.value; // These are uniformly distributed random numbers between 0 and 1
        float u2 = Random.value;

        // Box-Muller transform
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        // Convert the standard normal to the desired mean and standard deviation
        return mean + standardDeviation * randStdNormal;
    }
}
