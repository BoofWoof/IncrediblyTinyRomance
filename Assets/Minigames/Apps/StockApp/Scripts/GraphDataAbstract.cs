using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GraphDataAbstract : MonoBehaviour
{
    [Header("Stock ID")]
    public string StockName;
    public Sprite StockSprite;
    public Texture2D StockTexture;
    public Color StockColor;
    public Button AssociatedButton;

    [Header("Value Display")]
    public List<float> GraphValues = new List<float>();

    [Header("Stock Splitting")]
    public float MaxStockValue;
    public float MinStockValue;

    public delegate void StockSplitDelegate(string stockName);
    public StockSplitDelegate StockSplitEvent;
    public delegate void ReverseStockSplitDelegate(string stockName);
    public ReverseStockSplitDelegate ReverseStockSplitEvent;

    public void Awake()
    {
        StartUniqueGraphData();
        GenerateIntialValues();
    }

    public void OnEnable()
    {
        GraphScript.GraphStepEvent += Step;
    }

    public void OnDisable()
    {
        GraphScript.GraphStepEvent -= Step;
    }


    public void GenerateIntialValues() {
        while(GraphValues.Count < GraphScript.MaxLength)
        {
            GenerateNextValue();
        }
    }
    public virtual void SaveValues()
    {

    }
    public virtual void LoadValues()
    {

    }
    public virtual void Step()
    {
        GenerateNextValue();
        TrimData();
        if (GraphValues[GraphValues.Count-1] > MaxStockValue)
        {
            StockSplit();
        }
        if (GraphValues[GraphValues.Count - 1] < MinStockValue)
        {
            ReverseStockSplit();
        }
    }
    private void TrimData()
    {
        if(GraphValues.Count > GraphScript.MaxLength)
        {
            GraphValues = GraphValues.GetRange(GraphValues.Count - GraphScript.MaxLength, GraphScript.MaxLength);
        }
    }
    private void StockSplit()
    {
        StockSplitEvent?.Invoke(StockName);
        float valueChange = MaxStockValue/2f;

        for (int i = 0; i < GraphValues.Count; i++)
        {
            GraphValues[i] = GraphValues[i] - valueChange;
        }
        if(this == GraphScript.instance.GraphData)
        {
            GraphScript.instance.Redraw();
        }
    }
    private void ReverseStockSplit()
    {
        ReverseStockSplitEvent?.Invoke(StockName);
        float valueChange = MinStockValue;

        for (int i = 0; i < GraphValues.Count; i++)
        {
            GraphValues[i] = GraphValues[i] + valueChange;
        }
        if (this == GraphScript.instance.GraphData)
        {
            GraphScript.instance.Redraw();
        }
    }
    public abstract void StartUniqueGraphData();
    public abstract void GenerateNextValue();
}
