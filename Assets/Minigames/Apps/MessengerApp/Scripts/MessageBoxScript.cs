using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageBoxScript : MonoBehaviour
{
    public bool start_automatically = false;

    [Header("Objects")]
    public TextMeshProUGUI text_object;
    public RectTransform text_rect;
    public RectTransform message_background;
    public Image profile_image;

    public bool left_facing = false;

    [Header("Data")]
    public string default_message_text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce vel nisi nec diam dignissim dictum lobortis ac dolor. Phasellus vitae felis tortor.";

    [Header("Tuning")]
    public int border_width = 25;
    public int border_height = 15;
    public int minimum_width = 200;
    public int maximum_width = 750;
    public float stem_height = 200;

    private string displayed_message = "";
    private float line_height = 500;

    public Sprite secondary_backing;

    private void Start()
    {
        if (start_automatically)
        {
            StartCoroutine(CharacterProgression(default_message_text));
        }
    }
    public float GetFinalHeight(string message_text)
    {
        string previous_text = text_object.text;
        text_object.text = message_text;
        text_object.ForceMeshUpdate();
        text_object.text = previous_text;
        return text_object.GetRenderedValues().y + 2 * border_height + stem_height;
    }

    public void UseSecondaryBacking()
    {
        message_background.GetComponent<Image>().sprite = secondary_backing;
        stem_height = 0;

        Destroy(profile_image.gameObject.transform.parent.gameObject);
    }

    public void SwitchLeft()
    {
        left_facing = true;

        RectTransform backTransform = message_background.GetComponent<RectTransform>();
        backTransform.pivot = new Vector2(1, 1);
        backTransform.position = Vector3.zero;

        text_object.alignment = TextAlignmentOptions.TopRight;
        text_rect.pivot = new Vector2(1, 1);
        text_rect.anchorMax = new Vector2(1, 1);
        text_rect.anchorMin = new Vector2(1, 1);

        text_rect.sizeDelta = new Vector2(maximum_width, line_height);
        text_rect.localPosition = new Vector2(-border_width, -border_height);

        UseSecondaryBacking();
    }

    public void SetSprite(Sprite pfp_image)
    {
        profile_image.sprite = pfp_image;
    }

    public void InstantComplete(string message_text)
    {
        text_object.text = message_text;
        UpdateBackingSize();
        UpdatePFPPosition();
    }

    public void UpdatePFPPosition()
    {
        float pfpdrop = text_object.GetRenderedValues().y + 50f;
        profile_image.transform.parent.localPosition = message_background.localPosition + (Vector3.left * 90f) + (Vector3.down * (pfpdrop + stem_height));
    }

    public void UpdateBackingSize()
    {
        text_object.ForceMeshUpdate();

        text_rect.sizeDelta = new Vector2(maximum_width, line_height);
        if (left_facing)
        {
            text_rect.localPosition = new Vector2(-border_width, -border_height);
        } else
        {
            text_rect.localPosition = new Vector2(border_width, -border_height);
        }

        float largest_width = text_object.GetRenderedValues().x;
        float largest_height = text_object.GetRenderedValues().y;
        message_background.sizeDelta = new Vector2(largest_width + 2f * border_width, largest_height + 2f * border_height + stem_height);
    }

    public IEnumerator CharacterProgression(string message_text)
    {
        message_background.sizeDelta = new Vector2(minimum_width + 2f * border_width, line_height + 2f * border_height);
        text_rect.sizeDelta = new Vector2(maximum_width, line_height);
        text_rect.localPosition = new Vector2(border_width, -border_height);

        text_object.text = message_text;
        text_object.ForceMeshUpdate();
        float largest_width = text_object.GetRenderedValues().x;
        float largest_height = text_object.GetRenderedValues().y;
        line_height = largest_height / text_object.textInfo.lineCount;

        text_rect.sizeDelta = new Vector2(largest_width, line_height);

        while (message_text.Length > 0)
        {
            AdvanceLetter(ref message_text);
            text_object.ForceMeshUpdate();
            float current_height = text_object.GetRenderedValues().y;

            int line_count = text_object.textInfo.lineCount;
            float background_width = Math.Max(text_object.GetRenderedValues().x, minimum_width);
            if(line_count <= 1)
            {
                message_background.sizeDelta = new Vector2(background_width + 2f * border_width, current_height + border_height * 2f + stem_height);
            } else
            {
                message_background.sizeDelta = new Vector2(largest_width + 2f * border_width, current_height + border_height * 2f + stem_height);
            }
            UpdatePFPPosition();

            yield return new WaitForSeconds(MessagingVariables.TimePerCharacter);
        }
    }

    public char AdvanceLetter(ref string message_text)
    {
        char new_letter = message_text[0];
        displayed_message += new_letter;
        message_text = message_text.Remove(0, 1);
        text_object.text = displayed_message;
        text_object.ForceMeshUpdate();

        return new_letter;
    }
}
