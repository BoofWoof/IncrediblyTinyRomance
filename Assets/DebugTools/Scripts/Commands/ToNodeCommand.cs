using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "ToNodeCommand", menuName = "DebugCommands/ToNodeCommand")]
    public class ToNodeCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            OverworldPositionScript.GoTo(args[0], int.Parse(args[1]));

            return true;
        }
    }
}