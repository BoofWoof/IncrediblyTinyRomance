using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using DS;

public class MessageOptionsScript : MonoBehaviour
{
    public GameObject button_template;

    public void CreateButtons(Response[] responseOptions)
    {
        int buttionIdx = 0;
        foreach (Response response in responseOptions)
        {
            CreateButton(response.formattedText.text, buttionIdx);
            buttionIdx++;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    private void CreateButton(string text, int optionIdx)
    {
        GameObject new_button = Instantiate(button_template, transform);
        TMP_Text button_text = new_button.GetComponentInChildren<TMP_Text>();

        button_text.text = text;
        button_text.ForceMeshUpdate();

        LayoutElement LayoutInfo = button_text.GetComponent<LayoutElement>();
        float width = button_text.GetRenderedValues().x;
        if (width < LayoutInfo.preferredWidth) LayoutInfo.preferredWidth = width;
        LayoutRebuilder.ForceRebuildLayoutImmediate(button_text.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(new_button.GetComponent<RectTransform>());

        new_button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(optionIdx));
    }
    public void OnButtonClick(int optionIdx)
    {
        MessengerApp.instance.SelectOption(optionIdx);
        Destroy(gameObject.gameObject);
    }
}
