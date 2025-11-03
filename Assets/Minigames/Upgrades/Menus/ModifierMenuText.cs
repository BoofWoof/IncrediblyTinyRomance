using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModifierMenuText : MonoBehaviour
{
    public delegate void RewardModifier(ref float BaseValue);
    public List<RewardModifier> ModifiersToList;

    public float BaseValue;
    public string Units;
    public string BaseText;
    public string FinalText;

    public float UpdateFrequency = 0.5f;
    public float TimePassed = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Update()
    {
        TimePassed += Time.deltaTime;

        if (TimePassed <= UpdateFrequency) return;
        TimePassed = 0f;

        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

        string newText = BaseText + ": <sprite index=1>" + BaseValue.NumberToString(true);
        foreach (RewardModifier modifierType in ModifiersToList)
        {
            Delegate[] Delegates = modifierType.GetInvocationList();
            foreach (Delegate modifier in Delegates)
            {
                Debug.Log("A");
                if (!(modifier.Target is ValueModifierAbstract modifierScript)) continue;
                Debug.Log("B");
                newText += "\n" + modifierScript.ModifierDescription();
                Debug.Log(modifierScript.ModifierDescription());
            }
        }
        float reward = BaseValue;
        foreach (RewardModifier modifierType in ModifiersToList)
        {
            modifierType?.Invoke(ref reward);
        }

        newText += "\n" + FinalText + ": <sprite index=1>" + reward.NumberToString(true);

        text.text = newText;
    }
}
