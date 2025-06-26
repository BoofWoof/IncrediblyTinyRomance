using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public float BetMulti;
    public float LastDelta;

    public Button CollectBetButton;

    public static int BetsSet = 0;

    public bool TimerUp = false;

    public void SetBet(GraphDataAbstract associatedGraph, float betMulti, bool positiveBet)
    {
        BetsSet++;
        CollectBetButton.gameObject.SetActive(false);

        AssociatedGraph = associatedGraph;
        StockTypeImage.sprite = AssociatedGraph.StockSprite;
        BetAmount = AssociatedGraph.GraphValues[AssociatedGraph.GraphValues.Count - 1];
        GetComponent<Image>().color = AssociatedGraph.StockColor;

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

    public void Update()
    {
        if (TimerUp) return;

        float LastDelta = AssociatedGraph.GraphValues[AssociatedGraph.GraphValues.Count - 1] - BetAmount;
        StockChangeText.text = "<sprite index=1> " + LastDelta.NumberToString();
        if(LastDelta > 0)
        {
            StockChangeText.color = Color.green;
        } else
        {
            StockChangeText.color = Color.red;
        }
    }

    public void ShowCollectionButton()
    {
        TimerUp = true;

        CollectBetButton.gameObject.SetActive(true);
    }

    public void CollectBet()
    {
        Destroy(gameObject);
        BetsSet--;
    }
}
