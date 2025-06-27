using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetSettingScript : MonoBehaviour
{
    public Button SubmitButton;
    public Button IncreaseButton;
    public Button DecreaseButton;

    public Material ButtonGlow;

    public int BetDirection = 0;

    public int BetMagnitude = -9;
    public int MinimumBetMagnitude = -9;
    public int MaximumBetMagnitude = 9;
    public TextMeshProUGUI BetMagnitudeText;

    public TextMeshProUGUI BetCostText;
    public float BetCost;

    [Header("BetPrefab")]
    public GameObject BetMonitorPrefab;
    public RectTransform BetMonitorHolder;

    public TextPopupUI ErrorText;

    public void SubmitBet()
    {
        if (!CanSubmit(true)) return;

        CurrencyData.Credits -= BetCost;

        GameObject bmPrefab = Instantiate(BetMonitorPrefab);
        RectTransform rect = bmPrefab.GetComponent<RectTransform>();
        rect.parent = BetMonitorHolder;
        float drop = BetMonitor.BetsSet * (rect.sizeDelta.y + 9f);
        rect.localScale = Vector3.one * 0.5f;
        rect.localRotation = Quaternion.identity;
        rect.localPosition = Vector2.down * (9f + drop);
        rect.anchoredPosition = Vector2.down * (9f + drop) / 2f;

        BetMonitor bmScript = bmPrefab.GetComponent<BetMonitor>();
        bmScript.SetBet(GraphScript.instance.GraphData, Mathf.Pow(10, BetMagnitude), BetDirection > 0);

        Reset();
    }

    public void Start()
    {
        float betMult = Mathf.Pow(10, BetMagnitude);
        BetMagnitudeText.text = "X" + betMult.NumberToString();
        Reset();
    }

    public void Update()
    {
        float betMult = Mathf.Pow(10, BetMagnitude);
        BetCost = betMult * GraphScript.instance.GetLatestStockValue();

        if (CanSubmit())
        {
            SubmitButton.GetComponent<Image>().color = Color.green * 0.85f;

        } else
        {
            SubmitButton.GetComponent<Image>().color = Color.red * 0.85f;
        }

        if(BetDirection == 0)
        {
            float brightness = Mathf.Sin(Time.time * 2f * Mathf.PI) * 0.1f + 0.9f;
            IncreaseButton.GetComponent<Image>().color = Color.white * brightness;
            DecreaseButton.GetComponent<Image>().color = Color.white * brightness;
        }

        BetCostText.text = "Bet Cost: <sprite index=1> " + BetCost.NumberToString(true);
    }

    public void ChangeMagnitude(int magChange)
    {
        BetMagnitude = Mathf.Clamp(BetMagnitude + magChange, MinimumBetMagnitude, MaximumBetMagnitude);
        float betMult = Mathf.Pow(10, BetMagnitude);
        BetMagnitudeText.text = "X" + betMult.NumberToString();
    }

    public void SelectIncrease()
    {
        IncreaseButton.GetComponent<Image>().color = Color.white;
        DecreaseButton.GetComponent<Image>().color = Color.grey;
        IncreaseButton.GetComponent<Image>().material = ButtonGlow;
        IncreaseButton.GetComponent<Image>().material.SetFloat("_Intensity", 0.9f);
        DecreaseButton.GetComponent<Image>().material = null;
        BetDirection = 1;
    }

    public void SelectDecrease()
    {
        DecreaseButton.GetComponent<Image>().color = Color.white;
        IncreaseButton.GetComponent<Image>().color = Color.grey;
        IncreaseButton.GetComponent<Image>().material = null;
        DecreaseButton.GetComponent<Image>().material = ButtonGlow;
        DecreaseButton.GetComponent<Image>().material.SetFloat("_Intensity", 0.5f);
        BetDirection = -1;
    }

    public void Reset()
    {
        EventSystem.current.SetSelectedGameObject(null);

        DecreaseButton.GetComponent<Image>().color = Color.white;
        IncreaseButton.GetComponent<Image>().color = Color.white;
        IncreaseButton.GetComponent<Image>().material = null;
        DecreaseButton.GetComponent<Image>().material = null;
        BetDirection = 0;
    }

    private bool CanSubmit(bool submitError = false)
    {
        if (BetDirection == 0)
        {
            if(submitError) ErrorText.ShowInvalidClickFeedback("Select Higher or Lower");
            return false;
        }
        if (CurrencyData.Credits < BetCost)
        {
            if (submitError) ErrorText.ShowInvalidClickFeedback("Insufficient Funds");
            return false;
        } 

        return true;
    }
}
