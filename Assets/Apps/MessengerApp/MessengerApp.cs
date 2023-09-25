using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MessengerApp : MonoBehaviour
{
    [Header("Objects")]
    public GameObject message_box;
    public GameObject message_options;
    public RectTransform content_rect;

    [Header("Data")]
    public TextAsset message;

    [Header("Tuning")]
    public float message_buffer = 10;
    public float start_buffer = 50;
    public float end_buffer = 25;
    public float left_buffer = 200;
    public float right_buffer = 200;
    public float default_time_between_message = 1.0f;

    private float conversation_height;
    private List<string> textLines;

    // Start is called before the first frame update
    void Start()
    {
        conversation_height = start_buffer;
        textLines = message.text.Split('\n').ToList();

        StartCoroutine(MessageProgression());
    }

    public IEnumerator MessageProgression()
    {
        while (textLines.Count > 0) {
            GameObject new_message = Instantiate(message_box, Vector2.zero, Quaternion.identity);
            new_message.transform.parent = content_rect;
            new_message.transform.localScale = Vector3.one;
            new_message.transform.localPosition = new Vector2(left_buffer, -conversation_height);

            string message_text = textLines[0];
            string pattern = @"\[(.*?)\]";
            string pfp_name = Regex.Match(message_text, pattern).Value;
            if(pfp_name.Length > 0)
            {
                message_text = message_text.Replace(pfp_name, "");
            }
            pfp_name = pfp_name.Replace("[", "").Replace("]", "");

            MessageBoxScript message_info = new_message.GetComponent<MessageBoxScript>();
            textLines.RemoveAt(0);
            float final_message_height = message_info.GetFinalHeight(message_text);
            conversation_height += final_message_height;
            content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);

            message_info.SetSprite(pfp_name);

            yield return StartCoroutine(message_info.CharacterProgression(message_text));
            yield return new WaitForSeconds(default_time_between_message);
        }

        //MOVE THIS TO AN ACTUAL WORKING LOCATION
        GameObject option_message = Instantiate(message_options, content_rect);
        option_message.transform.localPosition = new Vector2(content_rect.rect.width - right_buffer, -conversation_height);
        option_message.transform.localScale = Vector3.one;
        conversation_height += option_message.GetComponent<MessageOptionsScript>().height;
        Debug.Log(content_rect.rect.width);
        content_rect.sizeDelta = new Vector2(content_rect.rect.width, conversation_height + end_buffer);

    }
    }
