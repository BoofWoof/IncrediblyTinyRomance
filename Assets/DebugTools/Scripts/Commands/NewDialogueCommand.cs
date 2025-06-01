using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "NewDialogueCommand", menuName = "DebugCommands/NewDialogueCommand")]
    public class NewDialogueCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string dialogueName = string.Join(" ", args);

            ConversationManagerScript.instance.StartDialogue(dialogueName);

            return true;
        }
    }
}