using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "NewPosterUnlockCommand", menuName = "DebugCommands/PosterUnlockCommand")]
    public class PosterUnlockCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            UnlockablesManager.UnlockAllPortraits();

            return true;
        }
    }
}