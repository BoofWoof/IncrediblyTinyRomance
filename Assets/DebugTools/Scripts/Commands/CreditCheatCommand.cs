using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "NewCreditCheat", menuName = "DebugCommands/CreditCheat")]
    public class CreditCheatCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            CurrenyData.Credits += float.Parse(args[0]);

            return true;
        }
    }
}