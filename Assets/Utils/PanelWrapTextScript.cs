using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelWrapTextScript : MonoBehaviour
{
    public Vector2 padding = new Vector4(16, 8);

    public void Tighten(TextMeshProUGUI targetText)
    {
        // RESIZE
        targetText.ForceMeshUpdate();

        Vector2 preferredSize = new Vector2(
            targetText.GetRenderedValues().x,
            targetText.GetRenderedValues().y
        );

        RectTransform rect = GetComponent<RectTransform>();

        rect.sizeDelta = preferredSize + padding;
        targetText.rectTransform.sizeDelta = preferredSize;

        //Reset Positions
        rect.parent = targetText.transform;
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        rect.anchoredPosition = Vector2.zero;

        rect.parent = targetText.transform.parent;

        targetText.rectTransform.parent = rect;
    }
}
