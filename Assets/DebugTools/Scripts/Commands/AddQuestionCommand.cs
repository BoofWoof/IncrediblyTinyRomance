using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "AddQuestionCommand", menuName = "DebugCommands/AddQuestionCommand")]
    public class AddQuestionCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            BigCameraPoint.QuestionsAvailable++;

            return true;
        }
    }
}