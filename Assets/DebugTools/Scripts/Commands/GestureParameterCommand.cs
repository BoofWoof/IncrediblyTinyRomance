using DebugTools.DeveloperConsole.Commands;
using UnityEngine;

[CreateAssetMenu(fileName = "GestureParameterCommand", menuName = "DebugCommands/GestureParameterCommand")]
public class GestureParameterCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        string CharacterName = args[0];

        string ParameterName = string.Join(" ", args[1..]);

        foreach (CharacterSpeechScript characterSpeechScript in CharacterSpeechScript.CharacterSpeechInstances)
        {
            characterSpeechScript.GestureParameter(CharacterName, ParameterName);
        }

        return true;
    }

}
