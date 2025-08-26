using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "ADUnlockCommand", menuName = "DebugCommands/ADUnlockCommand")]
    public class ADUnlockCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            AerialDefenseScript.Unlock();

            return true;
        }
    }
}