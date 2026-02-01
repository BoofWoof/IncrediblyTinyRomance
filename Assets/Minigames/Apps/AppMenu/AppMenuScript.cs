using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class AppMenuObjectData
{
    public Sprite AppIcon;
    public GameObject TargetApp;
    public bool Unlocked;
    public string AppName;
}

public class AppMenuScript : AppScript
{
    public static AppMenuScript Instance;

    public RectTransform MenuGridTarget;

    public List<AppMenuObjectData> AppDatas;
    public Dictionary<string, AppMenuObjectData> NameToApp;
    public Sprite EmptyAppSprite;

    public GameObject AppButtonPrefab;

    private void OnEnable()
    {
        NameToApp = new Dictionary<string, AppMenuObjectData>();
        foreach(AppMenuObjectData AppData in AppDatas)
        {
            NameToApp.Add(AppData.AppName.ToLower(), AppData);
        }

        Lua.RegisterFunction("UnlockApp", null, SymbolExtensions.GetMethodInfo(() => RevealApp("")));
        Instance = this;
        MakeButtons();
    }

    private void OnDisable()
    {
        ClearButtons();
    }

    public static void UnlockAllApps()
    {
        Instance.ClearButtons();
        foreach (AppMenuObjectData AppData in Instance.AppDatas)
        {
            AppData.Unlocked = true;
        }
        Instance.MakeButtons();
    }

    private void ClearButtons()
    {
        foreach(Transform child in MenuGridTarget)
        {
            Destroy(child.gameObject);
        }
    }

    public static void RevealApp(string AppName)
    {
        Instance.ClearButtons();
        if (Instance.NameToApp.ContainsKey(AppName.ToLower()))
        {
            Instance.NameToApp[AppName.ToLower()].Unlocked = true;
        }
        Instance.MakeButtons();
    }

    private void MakeButtons()
    {
        foreach (AppMenuObjectData AppData in AppDatas)
        {
            GameObject newButton = CreateButton(AppData);
            newButton.transform.parent = MenuGridTarget;
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localPosition = Vector3.zero;
            newButton.transform.localRotation = Quaternion.identity;
        }
    }

    private GameObject CreateButton(AppMenuObjectData appData)
    {
        GameObject newButton = Instantiate(AppButtonPrefab);
        Image image = newButton.GetComponent<Image>();
        TMP_Text imageText = newButton.GetComponentInChildren<TMP_Text>();
        if(appData.AppIcon != null && appData.Unlocked == true)
        {
            image.sprite = appData.AppIcon;
            imageText.text = appData.AppName;
        } else
        {
            newButton.GetComponent<Button>().interactable = false;
            imageText.text = "";
            image.sprite = EmptyAppSprite;
            return newButton;
        }

        Button button = newButton.GetComponent<Button>();

        EventTrigger eventTrigger = newButton.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((eventData) => { 
            PointerEventData pointerData = (PointerEventData)eventData;
            if (pointerData.button != PointerEventData.InputButton.Left) return;
            OnAppRelease(appData); 
        });
        eventTrigger.triggers.Add(entry);

        return newButton;
    }
    private void OnAppRelease(AppMenuObjectData appData)
    {
        appData.TargetApp.GetComponent<AppScript>().Show(gameObject);
        Hide(false);
    }
}
