using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class AngyRockScript : MonoBehaviour
{
    public TMP_Text ClockText;
    public TMP_Text ScoreText;

    private static float StartingTime;
    private static int Score = 0;
    public static bool ActiveRun = false;

    public void Start()
    {
        ScoreText.text = "Score: " + Score.ToString();

        if (ActiveRun)
        {
            UpdateTimer();
        }
    }

    public void Update()
    {
        if (!ActiveRun) return;
        UpdateTimer();
    }

    public void UpdateTimer()
    {
        float timePassed = TimePassed();
        int minutes = Mathf.FloorToInt(timePassed / 60);
        int seconds = Mathf.FloorToInt(timePassed % 60);
        ClockText.text = string.Format("{0:00}:{1:00}", 4 - minutes, 59 - seconds);

        if (TimePassed() > (60 * 5))
        {
            ActiveRun = false;
            ResetScore();
        }
    }

    public void Praise()
    {
        if(ActiveRun == true)
        {
            if (TimePassed() > 60) IncreaseScore();
            return;
        }
        ActiveRun = true;
        IncreaseScore();
    }

    private float TimePassed()
    {
        return Time.unscaledTime - StartingTime;
    }

    private void IncreaseScore()
    {
        StartingTime = Time.unscaledTime;
        Score += 1;
        ScoreText.text = "Score: " + Score.ToString();
    }

    public void ResetScore()
    {
        Score = 0;
        ScoreText.text = "Score: " + Score.ToString();
        ClockText.text = "0:00";
    }
}
