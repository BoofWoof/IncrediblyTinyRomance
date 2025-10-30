using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "ShutUpCommand", menuName = "DebugCommands/ShutUpCommand")]
    public class ShutUp : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            CharacterSpeechScript.BroadcastShutUp();
            DialogueManager.StopConversation();

            return true;
        }
    }
}