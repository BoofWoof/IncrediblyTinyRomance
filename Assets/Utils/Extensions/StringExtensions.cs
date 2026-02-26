using System.Collections.Generic;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using static UnityEngine.InputSystem.InputControlScheme.MatchResult;

public struct TimeMarker<T>
{
    public T data;
    public float timeSec;

    public TimeMarker(T data, float timeSec)
    {
        this.data = data;
        this.timeSec = timeSec;
    }
}
public class TimeList<T> : IEnumerable
{
    private List<TimeMarker<T>> _list = new List<TimeMarker<T>>();
    private int CurrentPhenomeIdx = 0;

    // Standard indexer
    public TimeMarker<T> this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public int GetSize()
    {
        return _list.Count;
    }

    // Assumes that you don't go backwards in time.
    public (TimeMarker<T> low, TimeMarker<T> high) GetNearestData(float time)
    {
        for (int i = CurrentPhenomeIdx; i < _list.Count; i++)
        {
            if (time < _list[i].timeSec)
            {
                CurrentPhenomeIdx = i - 1;
                break;
            }
            CurrentPhenomeIdx = i;
        }
        if (CurrentPhenomeIdx < 0) CurrentPhenomeIdx = 0;
        int nextPhenomeIdx = CurrentPhenomeIdx + 1;
        if (nextPhenomeIdx >= _list.Count) nextPhenomeIdx = CurrentPhenomeIdx;

        return (_list[CurrentPhenomeIdx], _list[nextPhenomeIdx]);
    }

    // Optional: expose List<T> methods
    public void Add(TimeMarker<T> item) => _list.Add(item);
    public int Count => _list.Count;
    public TimeMarker<T>[] ToArray() => _list.ToArray();
    public void Clear() => _list.Clear();

    // Implement IEnumerable<T> to allow foreach
    public IEnumerator<TimeMarker<T>> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public struct ChoiceBlock
{
    public string PreText;
    public Dictionary<string, string> ChoiceDictionary;
}
public static class StringExtensions
{
    private static readonly Regex choiceRegex = new Regex(@"<c=(.*?)>(.*?)(?=<c=|$)", RegexOptions.Singleline);
    public static ChoiceBlock SplitByChoices(this string input)
    {
        ChoiceBlock choices = new ChoiceBlock();
        choices.ChoiceDictionary = new Dictionary<string, string>();

        MatchCollection matches = choiceRegex.Matches(input);

        if(matches.Count == 0)
        {
            return choices;
        }

        string preText = input.Substring(0, matches[0].Index);
        choices.PreText = preText;

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            string choiceKey = match.Groups[1].Value;
            string responseValue = match.Groups[2].Value;

            choices.ChoiceDictionary[choiceKey] = responseValue;
        }
        return choices;
    }

    public static string CleanResourcePath(this string rawPath)
    {
        if (string.IsNullOrEmpty(rawPath)) return string.Empty;

        // Normalize slashes
        rawPath = rawPath.Replace("\\", "/");

        // Remove "Assets/" if present
        if (rawPath.StartsWith("Assets/"))
        {
            rawPath = rawPath.Substring("Assets/".Length);
        }

        // Remove "Resources/" if present
        int resourcesIndex = rawPath.IndexOf("Resources/");
        if (resourcesIndex >= 0)
        {
            rawPath = rawPath.Substring(resourcesIndex + "Resources/".Length);
        }

        // Remove file extension like ".asset" or ".prefab" if present
        if (rawPath.EndsWith(".asset"))
        {
            rawPath = rawPath.Substring(0, rawPath.Length - ".asset".Length);
        }
        else if (rawPath.EndsWith(".prefab"))
        {
            rawPath = rawPath.Substring(0, rawPath.Length - ".prefab".Length);
        }

        return rawPath;
    }

    public static TimeList<T> ToTimeMarkers<T>(this string text, string split, Func<string, T> valueParser, float fps)
    {
        TimeList<T> result = new TimeList<T>();
        if (text.Length == 0) return result;

        string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        float startingTimeSec = 0;
        string frame = lines[0].Split(split)[0];
        if (float.TryParse(frame.Trim(), out float rawStartingTimeSec))
        {
            startingTimeSec = rawStartingTimeSec / fps;
        }

        foreach (string line in lines)
        {
            int index = line.IndexOf(split);

            if (index >= 0)
            {
                string timeRaw = line.Substring(0, index);
                string dataRaw = line.Substring(index + split.Length);

                if (float.TryParse(timeRaw.Trim(), out float rawTimeSec))
                {
                    T value = valueParser(dataRaw.Trim());
                    result.Add(new TimeMarker<T>(value, rawTimeSec / fps - startingTimeSec));
                }
            }
        }

        return result;
    }

}