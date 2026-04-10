using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;

public class TurkHoleScript : MonoBehaviour
{
    public Vector2Int cord;
    public TurkCubeScript filledWith;

    public void FillHole(TurkCubeScript filler)
    {
        filledWith = filler;
    }

    public void EmptyHole()
    {
        filledWith = null;
    }

    public bool isFilled()
    {
        bool filled = filledWith != null;
        if (filled) {
            Image img = filledWith.GetComponent<Image>();
            img.material = TurkPuzzleScript.instance.ActiveConstMat;
        } 
        return filled;
    }
}
