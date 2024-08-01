using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DSMemory
{
    public static Dictionary<string, string> OptionMemory = new Dictionary<string, string>();
    public static Dictionary<string, float> ValueMemory = new Dictionary<string, float>();

    static public void PrintMemory()
    {
        foreach (KeyValuePair<string, string> kvp in OptionMemory)
        {
            Debug.Log(string.Format("Key: {0}, Value: {1}", kvp.Key, kvp.Value));
        }
        foreach (KeyValuePair<string, float> kvp in ValueMemory)
        {
            Debug.Log(string.Format("Key: {0}, Value: {1}", kvp.Key, kvp.Value));
        }
    }

    static public void InitializeVariable(DialogueOptionsVariable variable)
    {
        string variableID = variable.uniqueID;
        switch (variable.VariableType)
        {
            case VariableTypeEnum.Value:
                {
                    if (!ValueMemory.ContainsKey(variableID))
                    {
                        ValueMemory.Add(variableID, variable.StartingValue);
                    }
                    break;
                }
            case VariableTypeEnum.Option:
                {
                    if (!OptionMemory.ContainsKey(variableID))
                    {
                        OptionMemory.Add(variableID, variable.StartingUuid);
                    }
                    break;
                }
        }

    }
}
