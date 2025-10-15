using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "ShutUpCommand", menuName = "DebugCommands/ShutUpCommand")]
    public class ShutUp : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            DialogueManager.StopConversation();

            return true;
        }
    }
}