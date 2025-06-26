using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class NumberConversion
{
    public static string NumberToString(this float value, bool force3Figures = false)
    {
        float absValue = Mathf.Abs(value);

        string Format(float val)
        {
            if (!force3Figures)
                return val.ToString("G3"); // General format, max 3 significant digits

            // Calculate number of digits before the decimal
            int digitsBeforeDecimal = (val == 0) ? 0 : Mathf.Max(0, (int)Mathf.Floor(Mathf.Log10(Mathf.Abs(val))) + 1);
            int decimals = Mathf.Max(0, 3 - digitsBeforeDecimal);
            string format = "0." + new string('0', decimals);
            return val.ToString(format);
        }

        if (absValue >= 1_000_000_000_000 * 0.9999f)
            return Format(value / 1_000_000_000_000f) + " T";
        else if (absValue >= 1_000_000_000 * 0.9999f)
            return Format(value / 1_000_000_000f) + " G";
        else if (absValue >= 1_000_000 * 0.9999f)
            return Format(value / 1_000_000f) + " M";
        else if (absValue >= 1_000 * 0.9999f)
            return Format(value / 1_000f) + " k";
        else if (absValue >= 1 * 0.9999f)
            return Format(value) + " ";
        else if (absValue >= 0.001 * 0.9999f)
            return Format(value * 1_000f) + " m";
        else if (absValue >= 0.000_001 * 0.9999f)
            return Format(value * 1_000_000f) + " µ";
        else if (absValue >= 0.000_000_001 * 0.9999f)
            return Format(value * 1_000_000_000f) + " n";
        else
            return Format(value) + " ";
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
