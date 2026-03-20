using PixelCrushers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoad : MonoBehaviour
{
    public int SlotNumber = 0;
    public Button SaveButton;
    public Button LoadButton;
    public TMP_Text ErrorText;

    public void Update()
    {
        LoadButton.interactable = SaveSystem.HasSavedGameInSlot(SlotNumber);

        if (GameStateMonitor.isEventActive() || ConversationManagerScript.ConversationOngoing || MessageQueue.GetQueueLength() > 0)
        {
            ErrorText.text = "Finish your current dialogues before saving.";
            SaveButton.interactable = false;
            return;
        } else if (!PrayerScript.instance.JudgementActive)
        {
            ErrorText.text = "<b>THE GREAT RAM</b> must be judging you before you can save.";
            SaveButton.interactable = false;
            return;
        } else
        {
            ErrorText.text = "";
            SaveButton.interactable = true;
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveToSlot(SlotNumber);
    }

    public void LoadGame()
    {
        DaytaScript.ExternalSkipStart = true;
        SaveSystem.LoadFromSlot(SlotNumber);
    }
}
