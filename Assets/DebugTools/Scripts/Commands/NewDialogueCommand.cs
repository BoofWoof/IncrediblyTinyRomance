using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "NewDialogueCommand", menuName = "DebugCommands/NewDialogueCommand")]
    public class NewDialogueCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string dialogueName = string.Join(" ", args);
            MessageQueue.addDialogue(dialogueName);
            //ConversationManagerScript.instance.StartDialogue(dialogueName);

            return true;
        }
    }
}