using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    public interface IConsoleCommands
    {
        string CommandWord { get; }
        bool Process(string[] args);
    }
}
