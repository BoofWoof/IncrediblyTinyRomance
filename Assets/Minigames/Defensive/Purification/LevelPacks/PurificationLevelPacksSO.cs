using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PurificationLevelPacksSO", menuName = "PurificationGame/PurificationLevelPacksSO")]
public class PurificationLevelPacksSO : ScriptableObject
{
    public List<PurificationLevelSO> Levels;
}
