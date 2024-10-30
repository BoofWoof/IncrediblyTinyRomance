using System;
using UnityEngine;

public class TDAppScript : AppScript
{
    public static GameObject Level;

    public static Action<int> OnLivesChanged;

    public static int tdLives;
    public static int TDLives
    {
        get => tdLives;
        set
        {
            // Only trigger the callback if the value is actually changing
            if (tdLives != value)
            {
                tdLives = value;
                Debug.Log("New Life Count: " + tdLives);
                OnLivesChanged?.Invoke(tdLives); // Trigger the callback
            }
        }
    }
    public static int MaxTDLives = 1;

    public static Action<int> OnEnemyCountChanged;
    public static int enemyCount = 0;
    public static int EnemyCount
    {
        get => enemyCount;
        set
        {
            // Only trigger the callback if the value is actually changing
            if (enemyCount != value)
            {
                enemyCount = value;
                Debug.Log("New Enemy Count: " + enemyCount);
                OnEnemyCountChanged?.Invoke(enemyCount); // Trigger the callback
            }
        }
    }

    private void Awake()
    {
        Hide(true);
        RegisterInputActions();
    }
}
