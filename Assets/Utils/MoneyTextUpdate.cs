using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyTextUpdate : MonoBehaviour
{
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = "Credits: <sprite index=1>" + CurrencyData.Credits.NumberToString();
    }
}
