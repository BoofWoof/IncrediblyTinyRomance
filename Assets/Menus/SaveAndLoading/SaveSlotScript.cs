using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotScript : MonoBehaviour
{
    public Toggle ToggleDisplay;
    public int SlotID;

    public bool NewSaveSlot = false;

    public TMP_Text Description;

    public void Start()
    {
        Reset();
    }

    public void OnClick()
    {
        foreach(SaveSlotScript saveslot in SaveMenuScript.SaveSlotItems)
        {
            saveslot.Reset();
        }
        ToggleDisplay.isOn = true;

        SaveMenuScript.NewSaveSelected = NewSaveSlot;
        SaveMenuScript.instance.SelectedSaveSlot = SlotID;
    }

    public void SetSaveSlot(int SlotNumber)
    {
        SlotID = SlotNumber;

        SaveMenuScript.SaveMetadata metaData = SaveMenuScript.GetMetaData(SlotNumber);
        if (metaData == null) return;

        Description.text = 
            $"Save Slot: {metaData.Slot}\n" +
            $"Day: {metaData.Day}\n" +
            $"Time Played: {System.TimeSpan.FromSeconds(metaData.TimePlayed).ToString("h\\:mm\\:ss")}\n" +
            $"Credits: {metaData.Credits}";

        RebuildLayouts();
    }
    public void SetNewSaveSlot(int SlotNumber)
    {
        SlotID = SlotNumber;

        Description.text =
            $"<b>NEW</b> Save Slot: {SlotNumber}\n" +
            $"Day: {DayInfo.CurrentDay}\n" +
            $"Time Played: {System.TimeSpan.FromSeconds(EndOfDayScript.GetTimePassed()).ToString("h\\:mm\\:ss")}\n" +
            $"Credits: {CurrencyData.Credits}";

        NewSaveSlot = true;

        RebuildLayouts();
    }
    public void RebuildLayouts()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(
                (RectTransform)Description.transform
            );

        LayoutRebuilder.ForceRebuildLayoutImmediate(
                (RectTransform)Description.transform.parent
            );

        LayoutRebuilder.ForceRebuildLayoutImmediate(
                (RectTransform)Description.transform.parent.parent
            );
    }

    public void Reset()
    {
        ToggleDisplay.isOn = false;
    }

    public void OnEnable()
    {
        SaveMenuScript.SaveSlotItems.Add(this);
    }

    public void OnDisable()
    {
        SaveMenuScript.SaveSlotItems.Remove(this);
    }
}
