using PixelCrushers.DialogueSystem;
using UnityEngine;

public class RotationQuest1 : MonoBehaviour
{
    public bool Quest1Active = false;
    public bool Quest2Active = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Lua.RegisterFunction("StartRotationQuest1", this, SymbolExtensions.GetMethodInfo(() => StartRotationQuest1()));
        Lua.RegisterFunction("StartRotationQuest2", this, SymbolExtensions.GetMethodInfo(() => StartRotationQuest2()));

        CityPivot.OnSpin += OnSpin;
    }

    public void StartRotationQuest1()
    {
        QuestManager.ChangeQuest("Lookie");
        Quest1Active = true;
    }

    public void StartRotationQuest2()
    {
        QuestManager.ChangeQuest("Lookie 2");
        Quest2Active = true;
    }

    public void OnSpin(int Spins)
    {
        if (Quest1Active)
        {
            QuestManager.CompleteQuest("Lookie");
            ConversationManagerScript.instance.ForceNextDialogue();
            Quest1Active=false;
        }

        if (Quest2Active)
        {
            if(Mathf.Abs(Spins%4) == 2)
            {
                QuestManager.CompleteQuest("Lookie 2");
                ConversationManagerScript.instance.ForceNextDialogue();
                Quest2Active = false;
            }
        }
    }
}