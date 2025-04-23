using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScript : MonoBehaviour
{
    public GameObject LevelSpawnPoint;
    public List<GameObject> Levels;

    public int LevelIdx = 0;

    private Vector3 StartingPos;

    public void Start()
    {
        StartingPos = transform.localPosition;
        MakeLevel();
    }

    public void OnEnable()
    {
        TDAppScript.OnLivesChanged += CheckForDeath;
        TDAppScript.OnEnemyCountChanged += CheckForWin;
    }

    public void OnDisable()
    {
        TDAppScript.OnLivesChanged -= CheckForDeath;
        TDAppScript.OnEnemyCountChanged -= CheckForWin;
    }

    public void CheckForWin(int newEnemyCount)
    {
        if (newEnemyCount == 0)
        {
            Destroy(TDAppScript.Level);
            RevealMenu();
            MakeLevel();
            Debug.Log("TDWon");
        }
    }

    public void CheckForDeath(int newLifeValue)
    {
        if (newLifeValue == 0)
        {
            Destroy(TDAppScript.Level);
            RevealMenu();
            MakeLevel();
            Debug.Log("TDLost");
        }
    }

    public void StartLevel()
    {
        transform.localPosition = StartingPos + Vector3.up * 500;
        TDAppScript.TDLives = TDAppScript.MaxTDLives;
        TDAppScript.enemyCount = 0; //A direct set so callback isn't triggered.
        TDAppScript.Level.GetComponentInChildren<TDEnemyManagerScript>().StartLevel();
    }

    public void RevealMenu()
    {
        transform.localPosition = StartingPos;
    }

    public void MakeLevel()
    {
        Destroy(TDAppScript.Level);
        TDAppScript.Level = Instantiate(Levels[LevelIdx]);
        TDAppScript.Level.transform.position = LevelSpawnPoint.transform.position;
    }

    public void IncreaseLevelIdx()
    {
        if (LevelIdx >= Levels.Count - 1) return;
        LevelIdx++;
        MakeLevel();
    }

    public void DecreaseLevelIdx()
    {
        if (LevelIdx == 0) return;
        LevelIdx--;
        MakeLevel();
    }
}
