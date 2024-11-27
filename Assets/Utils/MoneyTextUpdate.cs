using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyTextUpdate : MonoBehaviour
{
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = "Titan Funds: $" + GameData.Money.NumberToString();
    }
}
