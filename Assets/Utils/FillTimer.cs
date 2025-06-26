using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FillTimer : MonoBehaviour
{
    public float TimerLength = 120f;
    public Vector2 FillRange = new Vector2(0, 0.75f);

    public float TimePassed;
    public Coroutine FillCoroutine;

    public UnityEvent FilledEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FillCoroutine = StartCoroutine(BeginFill());
    }

    IEnumerator BeginFill()
    {
        Image image = GetComponent<Image>();
        image.fillAmount = 0;

        while (TimePassed < TimerLength)
        {
            TimePassed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(FillRange.x, FillRange.y, TimePassed/TimerLength);
            yield return null;
        }

        image.fillAmount = 1;

        FillComplete();
    }

    public void FillComplete()
    {
        FilledEvent?.Invoke();
    }
}
