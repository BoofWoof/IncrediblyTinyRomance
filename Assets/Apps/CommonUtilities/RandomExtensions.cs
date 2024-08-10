using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions
{
    public static Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
