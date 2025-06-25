using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphScript : MonoBehaviour
{
    [Header("Data")]
    public GraphDataAbstract GraphData;

    [Header("Viewer Range")]
    public float VerticalRange;
    public float VerticalBuffer;

    private float CurrentRoof;
    private float CurrentFloor;

    private float PanelWidth;
    private float PanelHeight;

    [Header("Display Settings")]
    public static int MaxLength = 100;
    public float LineWidth = 0.0003f;

    public Color PositiveColor;
    public Color NegativeColor;
    public Color AverageValueColor;

    private List<GameObject> Lines = new List<GameObject>();
    private float LineSpacing;

    [Header("Update Rate")]
    public float SecondsPerUpdate = 1f;

    [Header("UI Display")]
    public Material LineMaterial;
    public RectTransform Indicator;
    public TextMeshProUGUI RoofText;
    public TextMeshProUGUI FloorText;
    public TextMeshProUGUI Money;
    public TextMeshProUGUI CompanyName;
    public TextMeshProUGUI CompanyValue;
    public Image GraphBackground;
    public Image GraphForeground;
    public Image GraphStockSprite;
    public Image BetSetStockSprite;
    public Image BetSetBackdrop;
    public Image AppBackground;
    public Image RenownBackground;
    public Image DetailBackground;
    public Image UpgradeButton;

    [Header("BuyQuantity")]
    public float BuyQuantity = 0.000_000_001f;

    public static GraphScript instance;
    public delegate void GraphStepDelegate();
    public static GraphStepDelegate GraphStepEvent;

    private bool RedrawSinceStep = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetDataSource(GraphData);
        StartCoroutine(GraphStepLoop());
    }

    public void SetDataSource(GraphDataAbstract newGraphData)
    {
        GraphData = newGraphData;
        Redraw();
        CompanyName.text = GraphData.StockName;
        GraphBackground.color = GraphData.StockColor;
        GraphForeground.color = GraphData.StockColor;

        RectTransform rect = GraphData.AssociatedButton.GetComponent<RectTransform>();
        Indicator.parent = rect;
        Indicator.localPosition = new Vector3(0, 60f, 0);
        Indicator.GetComponentInChildren<Image>().color = GraphData.StockColor;

        RenownBackground.color = GraphData.StockColor;
        DetailBackground.color = GraphData.StockColor;
        UpgradeButton.color = GraphData.StockColor;

        GraphStockSprite.sprite = GraphData.StockSprite;
        BetSetStockSprite.sprite = GraphData.StockSprite;
        BetSetBackdrop.color = GraphData.StockColor;

        AppBackground.materialForRendering.SetColor("_GradientColor2", GraphData.StockColor/2f);
        AppBackground.materialForRendering.SetTexture("_TileGrid", GraphData.StockTexture);

        RedrawSinceStep = false;
    }

    public void Redraw()
    {
        ClearGraph();
        GenerateInitialGraph();

        RedrawSinceStep = true;
    }

    public void ClearGraph()
    {
        while(Lines.Count != 0)
        {
            Destroy(Lines[0]);
            Lines.RemoveAt(0);
        }
    }

    public void GenerateInitialGraph()
    {
        PanelHeight = GetComponent<RectTransform>().sizeDelta[1];
        PanelWidth = GetComponent<RectTransform>().sizeDelta[0];
        LineSpacing = PanelWidth / (MaxLength - 1);

        UpdateRoofFloorValues(
            GetLatestStockValue() + VerticalRange / 2f,
            GetLatestStockValue() - VerticalRange / 2f,
            false
            );
        GenerateGraph();
    }

    private IEnumerator GraphStepLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(SecondsPerUpdate);

            GraphStepEvent?.Invoke();

            ShiftGraph();

            if (!RedrawSinceStep)
            {
                Destroy(Lines[0]);
                Lines.RemoveAt(0);
                CreateLine(GraphData.GraphValues[MaxLength - 2], GetLatestStockValue());
            }

            CompanyValue.text = "<sprite index=1> " + (Mathf.Round(GetLatestStockValue())).ToString(); 

            //Move this to text later.
            Money.text = "<sprite index=1> " + CurrenyData.Credits.NumberToString();

            RedrawSinceStep = false;
        }
    }

    private void GenerateGraph()
    {
        for (int i = 0; i < GraphData.GraphValues.Count - 1; i++)
        {
            CreateLine(GraphData.GraphValues[i], GraphData.GraphValues[i + 1]);
        }
    }

    private bool ShiftGraph()
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

        // Shift Graph Left
        Vector3 shiftVector = new Vector3(-LineSpacing, 0, 0);
        foreach (GameObject obj in Lines)
        {
            obj.transform.localPosition += shiftVector;
        }

        return UpdateRoofFloorValues(
            CurrentRoof - verticalShift,
            CurrentFloor - verticalShift
            );
    }

    private float RoundToNearest25(float value)
    {
        return Mathf.Round(value / 25.0f) * 25f;
    }

    private bool UpdateRoofFloorValues(float roof, float floor, bool allowReset = true)
    {
        if (floor < GraphData.MinStockValue)
        {
            roof = GraphData.MinStockValue + VerticalRange;
            floor = GraphData.MinStockValue;
        }
        if (roof > GraphData.MaxStockValue) {
            roof = GraphData.MaxStockValue;
            floor = GraphData.MaxStockValue - VerticalRange;
        }

        float newFloor = RoundToNearest25(floor);
        float newRoof = RoundToNearest25(roof);

        bool redraw = newFloor != CurrentFloor || newRoof != CurrentRoof;

        CurrentFloor = newFloor;
        FloorText.text = CurrentFloor.ToString("G3");
        CurrentRoof = newRoof;
        RoofText.text = CurrentRoof.ToString("G3");

        if (redraw && allowReset)
        {
            Redraw();
            return true;
        }

        return false;
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
        image.material = LineMaterial;
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
        return GraphData.GraphValues[GraphData.GraphValues.Count - 1];
    }
}
