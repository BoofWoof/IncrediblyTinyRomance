using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MessageBoxScript : MonoBehaviour
{
    public bool start_automatically = false;

    [Header("Objects")]
    public TextMeshProUGUI text_object;
    public RectTransform text_rect;
    public RectTransform message_background;
    public Image profile_image;

    [Header("Data")]
    public string default_message_text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce vel nisi nec diam dignissim dictum lobortis ac dolor. Phasellus vitae felis tortor.";

    [Header("Tuning")]
    public int border_width = 25;
    public int border_height = 15;
    public int minimum_width = 200;
    public int maximum_width = 750;
    public float time_per_character = 0.04f; //Make this a setting eventually.
    public float stem_height = 200;

    private string displayed_message = "";
    private float line_height = 500;
    private AsyncOperationHandle<Sprite> sprite_load_handle;

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
        return text_object.GetRenderedValues().y + 2 * border_height;
    }

    public void SetSprite(string pfp_name)
    {
        if (pfp_name == null)
        {
            return;
        }
        if (pfp_name.Length == 0)
        {
            return;
        }
        string character = "Kobold";
        sprite_load_handle = Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/PFPs/" + character + "/" + pfp_name + ".png");
        sprite_load_handle.Completed += SpriteLoadCompleted;
    }

    private void SpriteLoadCompleted(AsyncOperationHandle<Sprite> sprite_load_handle)
    {
        //sprite_load_handle.ToString();
        if (sprite_load_handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("No sprite exists for "  + ".");
            return;
        }
        profile_image.sprite = sprite_load_handle.Result;
        profile_image.enabled = true;
    }

    public IEnumerator CharacterProgression(string message_text)
    {
        message_background.sizeDelta = new Vector2(minimum_width + 2 * border_width, line_height + 2 * border_height);
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

            int line_count = text_object.textInfo.lineCount;
            float background_width = Math.Max(text_object.GetRenderedValues().x, minimum_width);
            if(line_count <= 1)
            {
                message_background.sizeDelta = new Vector2(background_width + 2 * border_width, line_count * line_height + 2 * border_height + stem_height);
            } else
            {
                message_background.sizeDelta = new Vector2(largest_width + 2 * border_width, line_count * line_height + 2 * border_height + stem_height);
            }

            yield return new WaitForSeconds(time_per_character);
        }
    }

    public char AdvanceLetter(ref string message_text)
    {
        char new_letter = message_text[0];
        displayed_message += new_letter;
        message_text = message_text.Remove(0, 1);
        text_object.text = displayed_message;
        text_object.ForceMeshUpdate();

        if (new_letter == '<')
        {
            while (new_letter != '>') { new_letter = AdvanceLetter(ref message_text); }
        }

        return new_letter;
    }

    private void OnDestroy()
    {
        if (sprite_load_handle.IsValid() && sprite_load_handle.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(sprite_load_handle);
        }
    }
}
