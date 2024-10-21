using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMaskScript : MonoBehaviour
{
    private float startingWidth;
    private float startingHeight;
    public float fadeDuration;
    public float delayStart = 0.5f;

    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        startingWidth = rectTransform.sizeDelta.x;
        startingHeight = rectTransform.sizeDelta.y;
    }

    public void StartScreen()
    {
        StartCoroutine(ChangeScreenState(startingWidth, startingHeight, delayStart));
    }

    public void ShutDownScreen()
    {
        StartCoroutine(ChangeScreenState(startingWidth, 0, 0));
    }
    private IEnumerator ChangeScreenState(float newWidth, float newHeight, float delay)
    {
        yield return new WaitForSeconds(delay);

        float timeElapsed = 0f;

        RectTransform rectTransform = GetComponent<RectTransform>();
        float currentWidth = rectTransform.sizeDelta.x;
        float currentHeight = rectTransform.sizeDelta.y;

        // Gradually crossfade over the fadeDuration
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / fadeDuration;

            float progressWidth = Mathf.Lerp(currentWidth, newWidth, progress); 
            float progressHeight = Mathf.Lerp(currentHeight, newHeight, progress); 

            rectTransform.sizeDelta = new Vector2(progressWidth, progressHeight);

            yield return null;
        }
        yield return null;
    }
}
