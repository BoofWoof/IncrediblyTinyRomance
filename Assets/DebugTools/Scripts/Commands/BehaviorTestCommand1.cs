using DebugTools.DeveloperConsole.Commands;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorTestCommand", menuName = "DebugCommands/BehaviorTestCommand")]
public class BehaviorTestCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        string CharacterName = args[0];

        string BehaviorName = string.Join(" ", args[1..]);

        foreach (OverworldBehavior overworldBehaviors in OverworldBehavior.OverworldBehaviors)
        {
            overworldBehaviors.ExecuteBehavior(CharacterName, BehaviorName, 0f);
        }

        return true;
    }

}
