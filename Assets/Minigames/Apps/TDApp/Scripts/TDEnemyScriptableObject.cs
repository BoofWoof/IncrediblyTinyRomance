using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TDLevel", menuName = "TowerDefense/Enemy", order = 1)]
public class TDEnemyScriptableObject : ScriptableObject
{
    public GameObject PrefabModel;
    public char AssociatedLetter;

    [Header("Stats")]
    public float MaxHealth = 1;
    public float Speed = 2;
}
