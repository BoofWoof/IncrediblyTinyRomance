using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AppMenuScript : AppScript
{
    public List<Sprite> AppSprites;
    public List<GameObject> Apps;

    public float horizontal_gap;
    public float vertical_gap;
    public float horizontal_count_max;
    public float vertical_count_max;

    private List<GameObject> appButtons = new List<GameObject>();

    private void OnEnable()
    {
        int appsAdded = 0;
        for (int y = 0;  y < vertical_count_max; y++)
        {
            for (int x = 0; x < horizontal_count_max; x++)
            {
                GameObject newButton = CreateButton(AppSprites[appsAdded], new Vector3(117 + horizontal_gap * x, 633 - vertical_gap * y, 0), appsAdded);
                appButtons.Add(newButton);
                appsAdded++;
                if (appsAdded >= AppSprites.Count) break;
            }
            if (appsAdded >= AppSprites.Count) break;
        }
    }

    private GameObject CreateButton(Sprite sprite, Vector3 position, int appIdx)
    {
        GameObject newButton = new GameObject("App Image");
        newButton.transform.SetParent(transform);

        RectTransform rectTransform = newButton.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 200);

        Image image = newButton.AddComponent<Image>();
        image.sprite = sprite;

        Button button = newButton.AddComponent<Button>();

        EventTrigger eventTrigger = newButton.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((eventData) => { OnAppRelease(appIdx); });
        eventTrigger.triggers.Add(entry);

        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);
        rectTransform.anchoredPosition = new Vector2(0f, 0f);

        rectTransform.localPosition = position;
        rectTransform.localScale = Vector2.one;
        rectTransform.localRotation = Quaternion.identity;

        return newButton;
    }
    private void OnAppRelease(int appIdx)
    {
        if (Apps.Count <= appIdx) return;
        Apps[appIdx].GetComponent<AppScript>().Show(gameObject);
        Hide();
    }
}
