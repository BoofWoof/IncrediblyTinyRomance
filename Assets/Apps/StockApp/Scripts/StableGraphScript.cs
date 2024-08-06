using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableGraphScript : GraphScript
{
    [Header("Graph Specific Variables")]
    public float trend = 120;
    public float meanIncrease = 0.002f;
    public float stdIncrease = 0.005f;
    public float stdNoise = 5f;

    public override float GenerateNextValue()
    {
        if (GraphValues.Count == 0) return trend;

        float valueChange = RandomUtility.GenerateGaussian(meanIncrease, stdIncrease);

        if (valueChange > 0)
        {
            trend *= 1 + valueChange;
        } else
        {
            trend *= 1 / (1 + Mathf.Abs(valueChange));
        }

        float noise = RandomUtility.GenerateGaussian(0, stdNoise);

        return trend + noise;
    }

    public override void StockSplitEvent()
    {
        base.StockSplitEvent();

        trend /= 2f;
    }

    public override void ReverseStockSplitEvent()
    {
        base.ReverseStockSplitEvent();

        trend *= 2f;
    }
}
