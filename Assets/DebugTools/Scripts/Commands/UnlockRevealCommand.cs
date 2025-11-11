using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "UnlockRevealCommand", menuName = "DebugCommands/UnlockRevealCommand")]
    public class UnlockRevealCommand : ConsoleCommand
    {
        public string MinigameName;

        public override bool Process(string[] args)
        {
            UpgradeScreenScript.BroadcastUpgradeReveal(MinigameName, float.Parse(args[0]));

            return true;
        }
    }
}