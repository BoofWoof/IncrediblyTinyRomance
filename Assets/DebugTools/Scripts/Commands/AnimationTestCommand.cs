using DebugTools.DeveloperConsole.Commands;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationTestCommand", menuName = "DebugCommands/AnimationTestCommand")]
public class AnimationTestCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        string CharacterName = args[0];

        string StateName = string.Join(" ", args[1..]);

        CharacterSpeechScript.BroadcastForceGesture(CharacterName, StateName);

        return true;
    }

}
