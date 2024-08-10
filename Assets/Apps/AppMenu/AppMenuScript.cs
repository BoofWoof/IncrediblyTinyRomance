using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AppMenuScript : AppScript
{
    public List<Sprite> AppSprites;
    public Sprite EmptyAppSprite;
    public List<GameObject> Apps;

    public float vertical_gap;
    public float horizontal_count_max;
    public float vertical_count_max;

    public RectTransform parent_panel;
    public float app_width = 200;
    public float vertical_buffer = 0;
    public float horizontal_buffer = 0;

    private List<GameObject> appButtons = new List<GameObject>();

    private void OnEnable()
    {
        int appsAdded = 0;

        float horizontal_gap = (parent_panel.sizeDelta.x - horizontal_buffer * 2f - app_width * horizontal_count_max) / (horizontal_count_max - 1f);
        for (int y = 0;  y < vertical_count_max; y++)
        {
            for (int x = 0; x < horizontal_count_max; x++)
            {
                float horizontal_position = app_width/2f + horizontal_buffer + (horizontal_gap + app_width) * x - parent_panel.sizeDelta.x / 2f;
                float vertical_position = vertical_buffer - vertical_gap * y;
                if (appsAdded < AppSprites.Count)
                {
                    GameObject newButton = CreateButton(AppSprites[appsAdded], new Vector2(horizontal_position, vertical_position), appsAdded);
                    appButtons.Add(newButton);
                    appsAdded++;
                } else
                {
                    GameObject newEmpty = CreateButton(EmptyAppSprite, new Vector2(horizontal_position, vertical_position), -1);
                }
            }
        }
    }

    private GameObject CreateButton(Sprite sprite, Vector2 position, int appIdx)
    {
        GameObject newButton = new GameObject("App Image");
        newButton.transform.SetParent(transform);

        RectTransform rectTransform = newButton.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(app_width, app_width);

        Image image = newButton.AddComponent<Image>();
        image.sprite = sprite;

        if (appIdx >= 0)
        {
            Button button = newButton.AddComponent<Button>();

            EventTrigger eventTrigger = newButton.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            entry.callback.AddListener((eventData) => { OnAppRelease(appIdx); });
            eventTrigger.triggers.Add(entry);
        }

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.localPosition = position;
        rectTransform.localScale = Vector2.one;
        rectTransform.localRotation = Quaternion.identity;

        return newButton;
    }
    private void OnAppRelease(int appIdx)
    {
        if (Apps.Count <= appIdx) return;
        Apps[appIdx].GetComponent<AppScript>().Show(gameObject);
        Hide(false);
    }
}
