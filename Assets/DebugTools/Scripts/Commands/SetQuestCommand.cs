using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "SetQuestCommand", menuName = "DebugCommands/SetQuestCommand")]
    public class SetQuestCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            QuestManager.SetQuestByIndex(int.Parse(args[0]));

            return true;
        }
    }
}