using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockAppScript : AppScript
{
    private void Awake()
    {
        Hide(true);
        RegisterInputActions();
    }
}
