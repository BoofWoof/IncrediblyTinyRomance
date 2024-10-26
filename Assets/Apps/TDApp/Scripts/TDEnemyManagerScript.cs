using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDEnemyManagerScript : MonoBehaviour
{
    public List<TDEnemyScriptableObject> enemies;

    private List<TDRoadBuilderScript> routeScripts;

    private int currentWave = 0;
    private Dictionary<char, TDEnemyScriptableObject> enemyMap = new Dictionary<char, TDEnemyScriptableObject>();

    public void Start()
    {
        routeScripts = GetComponentsInChildren<TDRoadBuilderScript>().ToList();

        foreach (TDRoadBuilderScript roadBuilderScript in routeScripts) {
            roadBuilderScript.DrawRoad();
        }

        foreach (TDEnemyScriptableObject enemy in enemies)
        {
            if (enemyMap.ContainsKey(enemy.AssociatedLetter))
            {
                throw new System.Exception("Enemies share associated letter.");
            }

            enemyMap[enemy.AssociatedLetter] = enemy;
        }

        foreach (TDRoadBuilderScript roadBuilderScript in routeScripts)
        {
            StartCoroutine(roadBuilderScript.SpawnWave(currentWave, enemyMap));
        }
    }
}
