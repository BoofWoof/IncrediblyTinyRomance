using UnityEngine;
using TMPro;

public class SubtitleScript : MonoBehaviour
{
    public static SubtitleScript instance;

    private TMP_Text SubtitleText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        SubtitleText = GetComponent<TMP_Text>();
    }

    public void SetText(string newText)
    {
        SubtitleText.text = newText;
    }
}
