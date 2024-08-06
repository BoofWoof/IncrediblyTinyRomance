using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphScript : MonoBehaviour
{
    [Header("Value Display")]
    public int MaxLength;
    public List<float> GraphValues;
    private float StocksOwned;
    private float AverageValue = 0;

    [Header("Stock Splitting")]
    public float MaxStockValue;
    public float MinStockValue;

    [Header("Viewer Range")]
    public float VerticalRange;
    public float VerticalBuffer;

    private float CurrentRoof;
    private float CurrentFloor;

    private float PanelWidth;
    private float PanelHeight;

    [Header("Display Settings")]
    public float LineWidth = 0.0003f;

    public Color PositiveColor;
    public Color NegativeColor;
    public Color AverageValueColor;

    private List<GameObject> Lines = new List<GameObject>();
    private float LineSpacing;
    private GameObject AverageValueLine;

    [Header("Update Rate")]
    private float TimePassed = 0;
    public float SecondsPerUpdate = 1f;

    [Header("UI Display")]
    public TextMeshProUGUI RoofText;
    public TextMeshProUGUI FloorText;
    public TextMeshProUGUI StocksOwnedText;
    public TextMeshProUGUI AveragePriceText;

    private void Start()
    {
        PanelHeight = GetComponent<RectTransform>().sizeDelta[1];
        PanelWidth = GetComponent<RectTransform>().sizeDelta[0];
        LineSpacing = PanelWidth / (MaxLength-1);

        GraphValues = new List<float>();
        for (int j = 0; j < MaxLength; j++)
        {
            GraphValues.Add(GenerateNextValue());
        }

        UpdateRoofFloorValues(
            GetLatestStockValue() + VerticalRange / 2f, 
            GetLatestStockValue() - VerticalRange / 2f
            );
        CreateAverageValueLine();
        GenerateGraph();
    }

    private void Update()
    {
        TimePassed += Time.deltaTime;
        if (TimePassed > SecondsPerUpdate)
        {
            GraphValues.Add(GenerateNextValue());
            TimePassed -= SecondsPerUpdate;

            StockSplit();
            ShiftGraph();

            Destroy(Lines[0]);
            Lines.RemoveAt(0);
            GraphValues.RemoveAt(0);
            CreateLine(GraphValues[MaxLength - 2], GetLatestStockValue());

            UpdateAverageValueLine();
        }
    }

    private void CreateAverageValueLine()
    {

        AverageValueLine = new GameObject("AverageValueLine", typeof(Image));
        AverageValueLine.transform.SetParent(transform);

        Image image = AverageValueLine.GetComponent<Image>();
        image.color = AverageValueColor;

        RectTransform rectTransform = AverageValueLine.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(PanelWidth, 3f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.localPosition = Vector3.zero;

        rectTransform.localScale = Vector3.one;
        rectTransform.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void UpdateAverageValueLine()
    {
        AverageValueLine.transform.localPosition = new Vector3(0, valueToYPosition(AverageValue), 0);
    }

    public void BuyStock()
    {
        if (GameData.Money < GetLatestStockValue()) return;
        UpdateStocksOwned(1);
        GameData.Money -= GetLatestStockValue();
    }

    public void SellStock()
    {
        if (StocksOwned < 1) return;
        UpdateStocksOwned(-1);
        GameData.Money += GetLatestStockValue();
    }

    private void UpdateStocksOwned(float change, bool updateAverageValue = true)
    {
        StocksOwned += change;
        StocksOwnedText.text = "STOCK OWNED: " + StocksOwned.ToString("G3");
        if (change > 0 && updateAverageValue)
        {
            AverageValue = (change / StocksOwned * GetLatestStockValue()) + ((StocksOwned - change) / StocksOwned * AverageValue);
            UpdateAveragePriceText();
        }
        //Debug.Log(GameData.Money);
    }

    private void UpdateAveragePriceText()
    {
        AveragePriceText.text = "AVG PRICE: " + AverageValue.ToString("G3");
    }

    virtual public float GenerateNextValue()
    {
        return Random.Range(-100, 100);
    }

    private void StockSplit()
    {
        float mostRecentValue = GetLatestStockValue();
        //Stock Split
        if (mostRecentValue > MaxStockValue)
        {
            UpdateStocksOwned(StocksOwned, false);
            float stockValueChange = mostRecentValue / 2f;
            GraphValues[GraphValues.Count - 1] -= stockValueChange;
            GraphValues[GraphValues.Count - 2] -= stockValueChange;

            AverageValue = AverageValue / 2f;
            UpdateAveragePriceText();

            foreach (GameObject obj in Lines)
            {
                obj.transform.localPosition -= new Vector3(0, stockValueChange * PanelHeight / VerticalRange, 0);
            }
            StockSplitEvent();
        }

        //Reverse Stock Split
        if (mostRecentValue < MinStockValue)
        {
            UpdateStocksOwned(- StocksOwned / 2f, false);
            float stockValueChange = mostRecentValue;
            GraphValues[GraphValues.Count - 1] += stockValueChange;
            GraphValues[GraphValues.Count - 2] += stockValueChange;

            AverageValue = AverageValue * 2f;
            UpdateAveragePriceText();

            foreach (GameObject obj in Lines)
            {
                obj.transform.localPosition += new Vector3(0, stockValueChange * PanelHeight / VerticalRange, 0);
            }
            ReverseStockSplitEvent();
        }
    }

    private void GenerateGraph()
    {
        for (int i = 0; i < GraphValues.Count - 1; i++)
        {
            CreateLine(GraphValues[i], GraphValues[i + 1]);
        }
    }

    private void ShiftGraph()
    {
        float mostRecentValue = GetLatestStockValue();

        // Shift Graph Vertically
        float verticalShift = 0;
        if (mostRecentValue > CurrentRoof - VerticalBuffer)
        {
            verticalShift = (CurrentRoof - VerticalBuffer) - mostRecentValue;
        } else if (mostRecentValue < CurrentFloor + VerticalBuffer)
        {
            verticalShift = (CurrentFloor + VerticalBuffer) - mostRecentValue;
        }

        UpdateRoofFloorValues(
            CurrentRoof - verticalShift,
            CurrentFloor - verticalShift
            );

        // Shift Graph Left
        Vector3 shiftVector = new Vector3(-LineSpacing, verticalShift * PanelHeight / VerticalRange, 0);
        foreach (GameObject obj in Lines)
        {
            obj.transform.localPosition += shiftVector;
        }
    }

    private void UpdateRoofFloorValues(float roof, float floor)
    {
        CurrentFloor = floor;
        FloorText.text = floor.ToString("G3");
        CurrentRoof = roof;
        RoofText.text = roof.ToString("G3");
    }
    private void ClearGraph()
    {
        foreach (GameObject obj in Lines)
        {
            if (obj != null) // Check if the object is not already destroyed
            {
                Destroy(obj);
            }
        }
        Lines.Clear();
    }

    private float valueToYPosition(float value)
    {
        float YPosition = (value - CurrentFloor) * PanelHeight / VerticalRange;
        return YPosition;
    }

    private GameObject CreateLine(float valueA, float valueB)
    {
        Vector2 PointA = new Vector2(LineSpacing * Lines.Count, valueToYPosition(valueA));
        Vector2 PointB = new Vector2(LineSpacing * (Lines.Count + 1), valueToYPosition(valueB));

        GameObject lineGO = new GameObject("Line", typeof(Image));
        lineGO.transform.SetParent(transform);

        Image image = lineGO.GetComponent<Image>();
        if (valueA < valueB)
        {
            image.color = PositiveColor;
        }
        else
        {
            image.color = NegativeColor;
        }

        RectTransform rectTransform = lineGO.GetComponent<RectTransform>();
        Vector2 dir = (PointB - PointA).normalized;
        float distance = Vector2.Distance(PointA, PointB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.localPosition = PointA + dir * distance * 0.5f;

        rectTransform.localScale = Vector3.one;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        Lines.Add(lineGO);

        return lineGO;
    }

    private float GetLatestStockValue()
    {
        return GraphValues[GraphValues.Count - 1];
    }

    virtual public void StockSplitEvent()
    {

    }

    virtual public void ReverseStockSplitEvent()
    {

    }
}
