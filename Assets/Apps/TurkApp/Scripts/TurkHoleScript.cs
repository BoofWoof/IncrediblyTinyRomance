using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using UnityEngine;

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
        return (filledWith != null);
    }
}
