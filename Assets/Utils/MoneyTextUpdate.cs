using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTextUpdate : MonoBehaviour
{
    string PrevText = "";
    void Update()
    {
        string CurrentText = "Credits: <sprite index=1> " + CurrencyData.Credits.NumberToString();
        if (CurrentText == PrevText) return;
        GetComponent<TextMeshProUGUI>().text = CurrentText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        PrevText = CurrentText;

    }
}
