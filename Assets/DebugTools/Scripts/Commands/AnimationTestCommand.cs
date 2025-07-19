using DebugTools.DeveloperConsole.Commands;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationTestCommand", menuName = "DebugCommands/AnimationTestCommand")]
public class AnimationTestCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        string CharacterName = args[0];

        string StateName = string.Join(" ", args[1..]);

        foreach (CharacterSpeechScript characterSpeechScript in CharacterSpeechScript.CharacterSpeechInstances)
        {
            characterSpeechScript.ForceGesture(CharacterName, StateName);
        }

        return true;
    }

}
