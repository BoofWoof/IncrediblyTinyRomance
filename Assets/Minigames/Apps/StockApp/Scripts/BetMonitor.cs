using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class BetMonitor : MonoBehaviour
{
    public GraphDataAbstract AssociatedGraph;

    public TextMeshProUGUI StockChangeText;
    public TextMeshProUGUI StockMultiText;

    public bool PositiveBet;
    public GameObject UpArrow;
    public GameObject DownArrow;

    public FillTimer Timer;

    public Image StockTypeImage;

    public float BetAmount;
    public float RawBetAmount;
    public float BetMulti;
    public float LastDelta;

    public Button CollectBetButton;

    public static int BetsSet = 0;
    public static List<BetMonitor> BetMonitorList = new List<BetMonitor>();
    public static float BetsRepositionPeriod = 0.5f;
    public static Coroutine RepositionCoroutine;
    public static BetMonitor CoroutineSource;

    public bool TimerUp = false;


    public void SetBet(GraphDataAbstract associatedGraph, float betMulti, bool positiveBet)
    {
        BetMonitorList.Add(this);

        BetsSet++;
        CollectBetButton.gameObject.SetActive(false);

        AssociatedGraph = associatedGraph;
        StockTypeImage.sprite = AssociatedGraph.StockSprite;
        BetAmount = AssociatedGraph.GraphValues[AssociatedGraph.GraphValues.Count - 1];
        RawBetAmount = BetAmount;
        GetComponent<Image>().color = AssociatedGraph.StockColor;

        AssociatedGraph.StockSplitEvent += ShiftBetValue;
        AssociatedGraph.ReverseStockSplitEvent += ShiftBetValue;

        BetMulti = betMulti;
        StockMultiText.text = "X" + BetMulti.NumberToString();

        PositiveBet = positiveBet;
        if (PositiveBet)
        {
            Destroy(DownArrow);
        }
        else
        {
            Destroy(UpArrow);
        }
    }

    public void ShiftBetValue(string shiftSource, float shiftAmount)
    {
        BetAmount += shiftAmount;
    }
    public void OnDisable()
    {
        AssociatedGraph.StockSplitEvent -= ShiftBetValue;
        AssociatedGraph.ReverseStockSplitEvent -= ShiftBetValue;
    }

    public void Update()
    {
        if (TimerUp) return;

        LastDelta = AssociatedGraph.GraphValues[AssociatedGraph.GraphValues.Count - 1] - BetAmount;
        StockChangeText.text = "<sprite index=1> " + LastDelta.NumberToString();
        if(LastDelta > 0)
        {
            StockChangeText.color = Color.green;
            if (PositiveBet)
            {
                UpArrow.GetComponent<Image>().color = Color.white;
            } else
            {
                DownArrow.GetComponent<Image>().color = Color.grey;
            }
        } else
        {
            StockChangeText.color = Color.red;
            if (PositiveBet)
            {
                UpArrow.GetComponent<Image>().color = Color.grey;
            }
            else
            {
                DownArrow.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void ShowCollectionButton()
    {
        TimerUp = true;

        bool positiveDelta = (LastDelta > 0);
        if (positiveDelta == PositiveBet)
        {
            CollectBetButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            CollectBetButton.GetComponent<Image>().color = Color.red;
        }

        CollectBetButton.gameObject.SetActive(true);
    }

    public void CollectBet()
    {
        BetMonitorList.Remove(this);

        StartRepositioningBets(this);

        BetsSet--;

        float currencyGain = (
            (RawBetAmount +
            Mathf.Abs(LastDelta))*
            BetMulti
            );
        float renownGain = (
            Mathf.Abs(LastDelta)*
            BetMulti
            );
        bool positiveDelta = (LastDelta > 0);
        if(PositiveBet == positiveDelta)
        {
            CurrencyData.Credits += currencyGain;
            CurrencyGet.GetRenown(AssociatedGraph.EnumName) += renownGain;
            GetComponent<TextPopupUI>().ShowInvalidClickFeedback(
                "+<sprite index=1> " + currencyGain.NumberToString() + "\n" +
                "+" + AssociatedGraph.StockName + " " + renownGain.NumberToString(),
                transform.parent.GetComponent<RectTransform>()
                );
        } else
        {
            GetComponent<TextPopupUI>().ShowInvalidClickFeedback("Bad Bet", transform.parent.GetComponent<RectTransform>());
        }
        Debug.Log(LastDelta);
        Debug.Log(PositiveBet);
        Debug.Log(positiveDelta);

        Destroy(gameObject);

    }
    public static void StartRepositioningBets(BetMonitor source)
    {
        if (BetMonitorList.Count <= 0) return;

        if (RepositionCoroutine != null && CoroutineSource != null) CoroutineSource.StopCoroutine(RepositionCoroutine);

        CoroutineSource = BetMonitorList[0];
        RepositionCoroutine = BetMonitorList[0].StartCoroutine(RepositionBetsCoroutine());
    }

    public static IEnumerator RepositionBetsCoroutine()
    {
        List<Vector2> startingAnchoredList = new List<Vector2>();
        foreach (BetMonitor betMonitor in BetMonitorList)
        {
            RectTransform rect = betMonitor.GetComponent<RectTransform>();
            startingAnchoredList.Add(rect.anchoredPosition);
        }

        float time = 0;

        while(time < BetsRepositionPeriod)
        {
            float t = time / BetsRepositionPeriod;
            int idx = 0;
            foreach(BetMonitor betMonitor in BetMonitorList)
            {
                RectTransform rect = betMonitor.GetComponent<RectTransform>();

                float drop = idx * (rect.sizeDelta.y + 9f);
                Vector2 drop2 = Vector2.down * (9f + drop) / 2f;
                rect.anchoredPosition = Vector2.Lerp(startingAnchoredList[idx], drop2, t);
                idx++;
            }
            time += Time.deltaTime;
            yield return null;
        }
    }
}
