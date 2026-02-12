using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "UnlockRevealCommand", menuName = "DebugCommands/UnlockRevealCommand")]
    public class UnlockRevealCommand : ConsoleCommand
    {
        public string MinigameName;

        public override bool Process(string[] args)
        {
            UpgradeHolder.UnlockAll(Minigame.Visions);
            return true;
        }
    }
}