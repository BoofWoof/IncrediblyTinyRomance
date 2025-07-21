using UnityEngine;

public class FlockGraph : GraphDataAbstract
{
    public float StartingValue = 100f;

    public float MeanValue;
    public float VarValue;

    public override void GenerateNextValue()
    {
        float valueDelta = RandomExtensions.Gaussian(MeanValue, VarValue);
        GraphValues.Add(GraphValues[GraphValues.Count-1] + valueDelta);
    }

    public override void StartUniqueGraphData()
    {
        GraphValues.Add(StartingValue);
    }
}
