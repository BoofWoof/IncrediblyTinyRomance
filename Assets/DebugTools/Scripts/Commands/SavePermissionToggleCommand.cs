using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "SavePermissionToggleCommand", menuName = "DebugCommands/SavePermissionToggleCommand")]
    public class SavePermissionToggleCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            SaveMenuScript.ToggleForceAllowSaves();

            return true;
        }
    }
}