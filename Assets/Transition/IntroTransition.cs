using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IntroTransition : MonoBehaviour
{
    public Image ScreenBlack;

    public float FadeInRate = 2f;
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float progress = 0;

        while(progress < FadeInRate)
        {
            yield return null;

            progress += Time.deltaTime;
            float volume = Mathf.Lerp(0f, 1f, progress/FadeInRate);
            AudioListener.volume = volume;

            float alpha = Mathf.Lerp(1f, 0f, progress / FadeInRate);
            ScreenBlack.color = new Color(0, 0, 0, alpha);
        }
        Destroy(gameObject);
    }
}
