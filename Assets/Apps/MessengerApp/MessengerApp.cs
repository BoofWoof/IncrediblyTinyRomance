using DS;
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
    private DSDialogue dialogue;

    // Start is called before the first frame update
    void Start()
    {
        conversation_height = start_buffer;

        dialogue = GetComponent<DSDialogue>();

        StartCoroutine(MessageProgression());
    }

    public IEnumerator MessageProgression()
    {
        while (!dialogue.isDone())
        {
            DSReturnValueInfo returnValue = dialogue.getReturnValue();
            if (returnValue.ReturnValueObject != null)
            {
                if (returnValue.TypeUuid == returnValue.ReturnValueObject.StateUuids[0])
                {
                    yield return new WaitForSeconds(returnValue.ReturnValue);
                }
            }
            yield return new WaitForSeconds(default_time_between_message);
            if (dialogue.isSingleOption())
            {
                GameObject new_message = Instantiate(message_box, Vector2.zero, Quaternion.identity);
                new_message.transform.parent = content_rect;
                new_message.transform.localScale = Vector3.one;
                new_message.transform.localPosition = new Vector2(left_buffer, -conversation_height);

                string message_text = dialogue.getText();
                string pattern = @"\[(.*?)\]";
                string pfp_name = Regex.Match(message_text, pattern).Value;
                if (pfp_name.Length > 0)
                {
                    message_text = message_text.Replace(pfp_name, "");
                }
                pfp_name = pfp_name.Replace("[", "").Replace("]", "");

                MessageBoxScript message_info = new_message.GetComponent<MessageBoxScript>();
                float final_message_height = message_info.GetFinalHeight(message_text);
                conversation_height += final_message_height;
                content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);

                //message_info.SetSprite(pfp_name);

                yield return StartCoroutine(message_info.CharacterProgression(message_text));
                yield return new WaitForSeconds(default_time_between_message);
                dialogue.setChoice("Next Dialogue");
                continue;
            }else if (dialogue.isMultipleOptions())
            {
                //MOVE THIS TO AN ACTUAL WORKING LOCATION
                GameObject option_message = Instantiate(message_options, content_rect);
                option_message.transform.localPosition = new Vector2(content_rect.rect.width - right_buffer, -conversation_height);
                option_message.transform.localScale = Vector3.one;
                MessageOptionsScript messageOptionScript = option_message.GetComponent<MessageOptionsScript>();
                messageOptionScript.options = dialogue.getChoices();
                messageOptionScript.CreateButtons();
                conversation_height += messageOptionScript.height;
                content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);

                yield return StartCoroutine(messageOptionScript.WaitForResponse());
                yield return new WaitForSeconds(default_time_between_message);
                dialogue.setChoice(messageOptionScript.message);
                continue;
            } else
            {
                break;
            }
        }
    }
}
