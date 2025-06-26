using TMPro;
using UnityEngine;
using System.Collections;

public class TextPopupUI : MonoBehaviour
{
    [Header("Prefab & Settings")]
    public TextMeshProUGUI feedbackTextPrefab; // Assign a TextMeshProUGUI prefab
    public Transform feedbackSpawnParent;      // Usually the canvas or a sub-object
    public float fadeDuration = 1.5f;          // Time to fade out
    public float raiseDistance = 30f;           // How far text raises.

    public void ShowInvalidClickFeedback(string message)
    {
        // Instantiate text
        TextMeshProUGUI instance = Instantiate(feedbackTextPrefab, feedbackSpawnParent);
        instance.text = message;

        Color c = instance.color;
        c.a = 1;
        instance.color = c;

        StartCoroutine(FadeAndDestroy(instance));
    }

    private IEnumerator FadeAndDestroy(TextMeshProUGUI text)
    {
        float time = 0f;
        Color originalColor = text.color;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            text.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0, Mathf.Lerp(0, raiseDistance, t));
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t));
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(text.gameObject);
    }
}
