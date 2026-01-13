using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementScript : MonoBehaviour
{
    public static AnnouncementScript instance;

    public TMP_Text AnnouncementText;
    public Image PanelImage;

    public float ExpandPeriod = 0.5f;
    public float HoldPeriod = 1f;
    public float FadePeriod = 1f;

    private float StartingPanelAlpha;
    private Vector2 StartingPanelSize;

    public void OnEnable()
    {
        instance = this;

        StartingPanelAlpha = PanelImage.color.a;
        StartingPanelSize = PanelImage.rectTransform.sizeDelta;

        PanelImage.color = new Color(0, 0, 0, 0);
        AnnouncementText.color = new Color(1, 1, 1, 0);
    }

    public static void StartAnnouncement(string announcementText)
    {
        Debug.Log(instance);
        instance.StartCoroutine(instance.AnnouncementCoroutine(announcementText));
    }

    public IEnumerator AnnouncementCoroutine(string announcementText)
    {
        AnnouncementText.text = announcementText;

        PanelImage.color = new Color(0, 0, 0, StartingPanelAlpha);
        AnnouncementText.color = new Color(1, 1, 1, 1);

        PanelImage.rectTransform.sizeDelta = new Vector2(0, StartingPanelSize.y);

        float timePassed = 0;
        while (timePassed < ExpandPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / ExpandPeriod;
            PanelImage.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, StartingPanelSize.x, progress), StartingPanelSize.y);
            yield return null;
        }

        yield return new WaitForSeconds(HoldPeriod);

        timePassed = 0;
        while (timePassed < FadePeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / FadePeriod;
            PanelImage.color = new Color(0, 0, 0, Mathf.Lerp(StartingPanelAlpha, 0, progress));
            AnnouncementText.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, progress));
            yield return null;
        }
        PanelImage.color = new Color(0, 0, 0, 0);
        AnnouncementText.color = new Color(1, 1, 1, 0);
    }
}
