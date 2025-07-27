using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "WalkToNodeCommand", menuName = "DebugCommands/WalkToNodeCommand")]
    public class WalkToNodeCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            OverworldPositionScript.StartWalkTo(args[0], int.Parse(args[1]));

            return true;
        }
    }
}