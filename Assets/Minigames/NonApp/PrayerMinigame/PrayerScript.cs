using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrayerScript : MonoBehaviour
{
    public TextAsset GoodPrayers;
    private string[] GoodLines = null;
    public TextAsset BadPrayers;
    private string[] BadLines = null;

    private float RamAngyLevel = 0;
    public float AngerRate = 1f;

    private int GoodIdx;

    public List<Button> ButtonText;
    public TMP_Text AngyDebug;

    public delegate void PrayerSubmittedCallback(bool GoodPrayer);
    public static event PrayerSubmittedCallback PrayerSubmitted;

    public static int GoodPrayerCount = 0;
    public static int BadPrayerCount = 0;
    public static int TotalPrayerCount = 0;

    public static bool IncreaseAnger = false;

    public float WaitForNextPrayerSec = 2f;

    public SplineMessageScript FireworkLauncher;
    public PrayerFireworkTextScript PrayerFireworkTextScript;

    private void Start()
    {
        ProcessPrayers();
        GenerateNewPrayers();
    }

    private void Update()
    {
        if (!IncreaseAnger) return;
        RamAngyLevel += Time.deltaTime * AngerRate;
        AngyDebug.text = "RamAngyLevel: " + RamAngyLevel.ToString("0");
    }
    public void SubmitAnswer(int answerIdx)
    {
        StartCoroutine(SubmitPrayer(answerIdx));
    }

    IEnumerator SubmitPrayer(int answerIdx)
    {
        FireworkLauncher.ActivateMessage();
        string answerText = ButtonText[answerIdx].GetComponentInChildren<TMP_Text>().text;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        TotalPrayerCount += 1;
        if (GoodIdx == answerIdx)
        {
            Debug.Log("You win!");
            RamAngyLevel -= 30f;
            if (RamAngyLevel < 0) RamAngyLevel = 0;
            PrayerSubmitted.Invoke(true);
            GoodPrayerCount++;
        }
        else
        {
            Debug.Log("Ram be angy! >:C");
            RamAngyLevel += 30f;
            if (RamAngyLevel > 100) RamAngyLevel = 100;
            PrayerSubmitted.Invoke(false);
            BadPrayerCount++;
        }
        for (int i = 0; i < ButtonText.Count; i++)
        {
            ButtonText[i].interactable = false;
        }

        yield return new WaitForSeconds(WaitForNextPrayerSec);

        GenerateNewPrayers();
        for (int i = 0; i < ButtonText.Count; i++)
        {
            ButtonText[i].GetComponent<MessageSendScript>().RestartMessage();
        }
    }

    private void ProcessPrayers()
    {
        GoodLines = GoodPrayers.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (GoodLines.Length < 1)
        {
            Debug.LogError("Not enough lines in the text file!");
            return;
        }
        BadLines = BadPrayers.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        // Ensure we have at least 3 lines to select from
        if (BadLines.Length < 3)
        {
            Debug.LogError("Not enough lines in the text file!");
            return;
        }
    }

    private void GenerateNewPrayers()
    {
        List<string> selectedGoodLine = GetRandomUniqueLines(GoodLines, 1);
        List<string> selectedBadLines = GetRandomUniqueLines(BadLines, 2);

        GoodIdx = Random.Range(0, 3);
        int badCount = 0;
        for (int i = 0; i < ButtonText.Count; i++)
        {
            if (GoodIdx == i)
            {
                ButtonText[i].GetComponentInChildren<TMP_Text>().text = selectedGoodLine[0];
                continue;
            }
            ButtonText[i].GetComponentInChildren<TMP_Text>().text = selectedBadLines[badCount];
            badCount++;
        }
    }

    private List<string> GetRandomUniqueLines(string[] lines, int count)
    {
        // Create a list to hold unique lines
        HashSet<int> selectedIndices = new HashSet<int>();
        List<string> result = new List<string>();

        // Randomly select unique indices
        while (selectedIndices.Count < count)
        {
            int randomIndex = Random.Range(0, lines.Length);
            if (selectedIndices.Add(randomIndex)) // Add returns false if already present
            {
                result.Add(lines[randomIndex]);
            }
        }

        return result;
    }
}
