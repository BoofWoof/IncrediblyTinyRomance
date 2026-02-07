using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageBoxScript : MonoBehaviour
{
    public GameObject ReplacementMessageBox;

    [Header("Objects")]
    public TMP_Text text_object;
    public Image profile_image;

    [Header("Data")]
    public string default_message_text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce vel nisi nec diam dignissim dictum lobortis ac dolor. Phasellus vitae felis tortor.";

    [Header("Tuning")]
    public int minimum_width = 200;

    public Sprite secondary_backing;

    public void ReplaceBox()
    {
        int index = transform.GetSiblingIndex();

        GameObject replacementObject = Instantiate(ReplacementMessageBox, transform.parent);
        replacementObject.transform.SetSiblingIndex(index);
        replacementObject.transform.localPosition = Vector3.zero;
        replacementObject.transform.localRotation = Quaternion.identity;
        replacementObject.transform.localScale = Vector3.one;

        replacementObject.GetComponent<MessageBoxScript>().SetText(text_object.text);

        transform.parent = null;

        Destroy(gameObject);
    }

    public void SetSprite(Sprite pfp_image)
    {
        profile_image.sprite = pfp_image;
    }

    public void SetText(string newText)
    {
        text_object.text = newText;
        UpdateWidth();
    }

    public void UpdateWidth()
    {
        LayoutElement LayoutInfo = text_object.GetComponent<LayoutElement>();

        text_object.ForceMeshUpdate();

        float width = text_object.GetPreferredValues().x;

        if (width < LayoutInfo.preferredWidth) LayoutInfo.preferredWidth = width;
        LayoutRebuilder.ForceRebuildLayoutImmediate(text_object.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(text_object.transform.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}
