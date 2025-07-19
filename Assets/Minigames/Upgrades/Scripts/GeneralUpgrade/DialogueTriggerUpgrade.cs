using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTriggerUpgrade", menuName = "Upgrades/DialogueTriggerUpgrade")]
public class DialogueTriggerUpgrade : UpgradesAbstract
{
    public string DialogueName;
    public override void OnBuy()
    {
        ConversationManagerScript.instance.StartDialogue(DialogueName);
    }
}
