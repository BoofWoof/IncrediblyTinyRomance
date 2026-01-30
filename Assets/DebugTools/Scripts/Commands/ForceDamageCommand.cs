using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "ForceDamageCommand", menuName = "DebugCommands/ForceDamageCommand")]
    public class ForceDamageCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            DefenseStats.DamageCity(float.Parse(args[0]));

            return true;
        }
    }
}