using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBoxScript : MonoBehaviour
{
    [Header("Objects")]
    public TextMeshProUGUI text_object;
    public RectTransform text_rect;
    public RectTransform message_background;

    [Header("Data")]
    public string message_text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce vel nisi nec diam dignissim dictum lobortis ac dolor. Phasellus vitae felis tortor.";

    [Header("Tuning")]
    public int border_width = 10;
    public int border_height = 10;
    public int minimum_width = 500;
    public int maximum_width = 2000;
    public float time_per_character = 0.02f; //Make this a setting eventually.

    private string displayed_message = "";
    private float line_height = 500;

    private void Start()
    {
        message_background.sizeDelta = new Vector2(minimum_width + 2 * border_width, line_height + 2 * border_height);
        text_rect.sizeDelta = new Vector2(maximum_width, line_height);
        text_rect.localPosition = new Vector2(border_width, -border_height);

        text_object.text = message_text;
        text_object.ForceMeshUpdate();
        float largest_width = text_object.GetRenderedValues().x;
        float largest_height = text_object.GetRenderedValues().y;
        line_height = largest_height/ text_object.textInfo.lineCount;

        text_rect.sizeDelta = new Vector2(largest_width, line_height);

        StartCoroutine(CharacterProgression());
    }

    public IEnumerator CharacterProgression()
    {
        while (message_text.Length > 0)
        {
            AdvanceLetter();

            int line_count = text_object.textInfo.lineCount;
            float background_width = Math.Max(text_object.GetRenderedValues().x, minimum_width);
            message_background.sizeDelta = new Vector2(background_width + 2 * border_width, line_count * line_height + 2 * border_height);

            yield return new WaitForSeconds(time_per_character);
        }
    }

    public char AdvanceLetter()
    {
        char new_letter = message_text[0];
        displayed_message += new_letter;
        message_text = message_text.Remove(0, 1);
        text_object.text = displayed_message;
        text_object.ForceMeshUpdate();

        if (new_letter == '<')
        {
            while (new_letter != '>') { new_letter = AdvanceLetter(); }
        }

        return new_letter;
    }
}
