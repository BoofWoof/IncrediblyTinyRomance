using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrayerScript : MonoBehaviour
{
    public TextAsset GoodPrayers;
    private string[] GoodLines = null;
    public TextAsset BadPrayers;
    private string[] BadLines = null;

    private float RamAngyLevel = 0;
    public float AngerRate = 1f;

    private int GoodIdx;

    public List<TMP_Text> ButtonText;
    public TMP_Text AngyDebug;


    private void Start()
    {
        ProcessPrayers();
        GenerateNewPrayers();
    }

    private void Update()
    {
        RamAngyLevel += Time.deltaTime * AngerRate;
        AngyDebug.text = "RamAngyLevel: " + RamAngyLevel.ToString("0");
    }
    public void SubmitAnswer(int answerIdx)
    {
        if (GoodIdx == answerIdx)
        {
            Debug.Log("You win!");
            RamAngyLevel -= 30f;
            if (RamAngyLevel < 0) RamAngyLevel = 0;
        }
        else
        {
            Debug.Log("Ram be angy! >:C");
            RamAngyLevel += 30f;
            if (RamAngyLevel > 100) RamAngyLevel = 100;
        }
        GenerateNewPrayers();
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
        List<string> selectedBadLines = GetRandomUniqueLines(BadLines, 3);

        GoodIdx = Random.Range(0, 4);
        int badCount = 0;
        for (int i = 0; i < ButtonText.Count; i++)
        {
            if (GoodIdx == i)
            {
                ButtonText[i].text = selectedGoodLine[0];
                continue;
            }
            ButtonText[i].text = selectedBadLines[badCount];
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
