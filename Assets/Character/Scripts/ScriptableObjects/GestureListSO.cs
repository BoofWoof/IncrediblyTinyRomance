using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GestureListSO", menuName = "Scriptable Objects/GestureListSO")]
public class GestureListSO : ScriptableObject
{
    public List<string> ValidGestures;
}
