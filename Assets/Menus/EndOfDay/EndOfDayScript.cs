using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using TMPro;
using UnityEngine;

public class EndOfDayScript : MonoBehaviour
{
    public GameObject StatScreen;
    public TMP_Text StatText;

    public float StartingTime;
    public float StartingMiloLike;
    public float StartingAriesLike;

    public AudioSource EndOfDaySound;

    public void Start()
    {
        StartingTime = Time.time;
        StartingMiloLike = DialogueLua.GetVariable("MiloLike").asFloat;
        StartingAriesLike = DialogueLua.GetVariable("AriesLike").asFloat;

        StatScreen.SetActive(false);
    }

    public void GoSceneNextDay()
    {

    }
    public void GoSceneMenu()
    {
        SaveSystem.LoadScene("MainMenu");
    }

    public void OnEnable()
    {
        UpdateText();
    }

    public void ToggleStatActive()
    {
        StartCoroutine(BeginEndScreen());
    }

    public IEnumerator BeginEndScreen()
    {
        yield return new WaitForSeconds(2f);

        EndOfDaySound.Play();
        PhonePositionScript.LockPhoneDown();
        StatScreen.SetActive(true);
        UpdateText();
        InputManager.GameEnd();
    }

    public void UpdateText()
    {
        string totalDayTime = System.TimeSpan.FromSeconds(Time.time - StartingTime).ToString("h\\:mm\\:ss");

        string totalVentTime = System.TimeSpan.FromSeconds(PurificationGameScript.TotalTime).ToString("m\\:ss");

        string FastestVeryEasy = "";
        if (TurkPuzzleScript.TimeRecords.ContainsKey(0))
        {
            FastestVeryEasy = System.TimeSpan.FromSeconds(TurkPuzzleScript.TimeRecords[0]).ToString("m\\:ss");
        }
        string FastestEasy = "";
        if (TurkPuzzleScript.TimeRecords.ContainsKey(1))
        {
            FastestEasy = System.TimeSpan.FromSeconds(TurkPuzzleScript.TimeRecords[1]).ToString("m\\:ss");
        }
        string FastestNormal = "";
        if (TurkPuzzleScript.TimeRecords.ContainsKey(2))
        {
            FastestNormal = System.TimeSpan.FromSeconds(TurkPuzzleScript.TimeRecords[2]).ToString("m\\:ss");
        }
        string FastestHard = "";
        if (TurkPuzzleScript.TimeRecords.ContainsKey(3))
        {
            FastestHard = System.TimeSpan.FromSeconds(TurkPuzzleScript.TimeRecords[3]).ToString("m\\:ss");
        }
        int puzzlesSolved = 0;
        foreach (int puzzleCount in TurkPuzzleScript.PuzzlesCompleted.Values)
        {
            puzzlesSolved += puzzleCount;
        }

        float MiloDelta = DialogueLua.GetVariable("MiloLike").asFloat - StartingMiloLike;
        float AriesDelta = DialogueLua.GetVariable("AriesLike").asFloat - StartingAriesLike;

        StatText.text =
            "<b>Total Time:</b> " + totalDayTime + "\r\n" +
            "<b>Time Spent Solving Vents:</b> " + totalVentTime + "\r\n" +
            "<b>Fastest Very Easy Puzzle:</b> " + FastestVeryEasy + "\r\n" +
            "<b>Fastest Easy Puzzle:</b> " + FastestEasy + "\r\n" +
            "<b>Fastest Normal Puzzle:</b> " + FastestNormal + "\r\n" +
            "<b>Fastest Hard Puzzle:</b> " + FastestHard + "\r\n" +
            "\r\n" +
            "<b>Prayers Sent:</b>" + PrayerScript.TotalPrayerCount.ToString() + "\r\n" +
            "<b>Puzzles Solved:</b> " + puzzlesSolved.ToString() + "\r\n" +
            "<b>Posters Unlocked:</b> " + UnlockablesManager.PostersUnlockedCount.ToString() + "\r\n" +
            "<b>Credits Earned:</b> " + "<sprite index=1>" + CurrencyData.Credits.NumberToString() + "\r\n" +
            "\r\n" +
            "<b>Milo Appreciation Change:</b> " + MiloDelta.ToString() + "\r\n" +
            "<b>Aries Spite Change:</b> " + AriesDelta.ToString();
    }
}
