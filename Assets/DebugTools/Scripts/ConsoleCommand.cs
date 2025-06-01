using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommands
    {
        [SerializeField] private string commandWord = string.Empty;
        public string CommandWord => commandWord;

        public abstract bool Process(string[] args);
    }
}
