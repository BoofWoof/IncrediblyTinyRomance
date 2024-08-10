using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class NumberConversion
{
    public static string NumberToString(this float value)
    {
        if (value >= 1_000_000_000_000 * 0.9999f)
        {
            return (value / 1_000_000_000_000.0).ToString("0.") + " T";
        }
        else if (value >= 1_000_000_000 * 0.9999f)
        {
            return (value / 1_000_000_000.0).ToString("0.") + " G";
        }
        else if (value >= 1_000_000 * 0.9999f)
        {
            return (value / 1_000_000.0).ToString("0.") + " M";
        }
        else if (value >= 1_000 * 0.9999f)
        {
            return (value / 1_000.0).ToString("0.") + " k";
        }
        else if (value >= 1 * 0.9999f)
        {
            return value.ToString("0.") + " ";
        }
        else if (value >= 0.001 * 0.9999f)
        {
            return (value * 1_000.0).ToString("0.") + " m";
        }
        else if (value >= 0.000_001 * 0.9999f)
        {
            return (value * 1_000_000.0).ToString("0.") + " µ";
        }
        else if (value >= 0.000_000_001 * 0.9999f)
        {
            return (value * 1_000_000_000.0).ToString("0.") + " n";
        }
        else
        {
            return value.ToString("0.") + " ";
        }
    }

    public static float RoundToSignificantFigures(this float num, int n)
    {
        if (num == 0)
        {
            return 0;
        }

        float d = Mathf.Ceil(Mathf.Log10(num < 0 ? -num : num));
        int power = n - (int)d;

        float magnitude = Mathf.Pow(10, power);
        float shifted = Mathf.Round(num * magnitude);
        return shifted / magnitude;
    }
}
