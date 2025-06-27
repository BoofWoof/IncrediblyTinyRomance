using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextPopupUI : MonoBehaviour
{
    [Header("Prefab & Settings")]
    public GameObject BackPanel;

    public TextMeshProUGUI feedbackTextPrefab; // Assign a TextMeshProUGUI prefab
    public Transform feedbackSpawnParent;      // Usually the canvas or a sub-object
    public float fadeDuration = 1.5f;          // Time to fade out
    public float raiseDistance = 30f;           // How far text raises.

    public bool postCenter = false;

    public void ShowInvalidClickFeedback(string message, RectTransform parentOverride = null)
    {

        // Instantiate text
        TextMeshProUGUI instance = Instantiate(feedbackTextPrefab, feedbackSpawnParent);
        instance.text = message;

        Color c = instance.color;
        c.a = 1;
        instance.color = c;

        GameObject backPanel = Instantiate(BackPanel);
        PanelWrapTextScript textWrapScript = backPanel.GetComponent<PanelWrapTextScript>();
        textWrapScript.Tighten(instance);

        if (postCenter) instance.alignment = TextAlignmentOptions.Center;

        if (parentOverride != null) {backPanel.transform.parent = parentOverride; }

        instance.StartCoroutine(FadeAndDestroy(backPanel, instance));
    }

    private IEnumerator FadeAndDestroy(GameObject panel, TextMeshProUGUI text)
    {
        float time = 0f;
        Color originalColor = text.color;

        Vector2 startingAncorchedPosition = panel.GetComponent<RectTransform>().anchoredPosition;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            panel.GetComponent<RectTransform>().anchoredPosition = startingAncorchedPosition + new Vector2 (0, Mathf.Lerp(0, raiseDistance, t));
            panel.GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Lerp(1, 0, t));
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t));
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(text.gameObject);
        Destroy(panel);
    }
}
