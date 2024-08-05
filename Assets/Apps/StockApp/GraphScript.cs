using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphScript : MonoBehaviour
{ 
    public int MaxLength;
    public List<float> GraphValues;

    public float LineWidth = 0.0003f;

    public Color PositiveColor;
    public Color NegativeColor;

    private List<GameObject> Lines = new List<GameObject>();
    private float LineSpacing;

    private float TimePassed = 0;

    private void Start()
    {
        LineSpacing = GetComponent<RectTransform>().sizeDelta[0] / (MaxLength-1);

        GraphValues = new List<float>();
        for (int j = 0; j < MaxLength; j++)
        {
            GraphValues.Add(Random.Range(-100, 100));
        }

        GenerateGraph();
    }

    private void Update()
    {
        TimePassed += Time.deltaTime;
        if (TimePassed > 1f)
        {
            TimePassed -= 1f;
            ShiftGraphLeft();
        }
    }

    private void GenerateGraph()
    {
        for (int i = 0; i < GraphValues.Count - 1; i++)
        {
            CreateLine(GraphValues[i], GraphValues[i + 1]);
        }
    }

    private void ShiftGraphLeft()
    {
        Vector3 shiftVector = new Vector3(LineSpacing, 0, 0);
        foreach (GameObject obj in Lines)
        {
            obj.transform.localPosition -= shiftVector;
        }
        Destroy(Lines[0]);
        Lines.RemoveAt(0);
        GraphValues.RemoveAt(0);
        GraphValues.Add(Random.Range(-100, 100));
        CreateLine(GraphValues[MaxLength-2], GraphValues[MaxLength-1]);
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

    private GameObject CreateLine(float valueA, float valueB)
    {
        Vector2 PointA = new Vector2(LineSpacing * Lines.Count, valueA);
        Vector2 PointB = new Vector2(LineSpacing * (Lines.Count + 1), valueB);

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
}
