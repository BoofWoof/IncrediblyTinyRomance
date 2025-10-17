using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    public GameObject BaseObject;

    public bool ContinueTutorial = true;
    public bool ContinueDialogue = false;
    public bool StartOff = false;

    public string ColliderName;

    public static List<TutorialCollider> TutorialColliders;

    public void Awake()
    {
    }

    public void Start()
    {
        if (TutorialColliders == null) TutorialColliders = new List<TutorialCollider>();

        TutorialColliders.Add(this);
        Lua.RegisterFunction("BroadcastActivateColliderByName", null, SymbolExtensions.GetMethodInfo(() => BroadcastActivateColliderByName("")));
        
        if (StartOff)
        {
            BaseObject.SetActive(false);
        }
    }

    public static void BroadcastActivateColliderByName(string colliderName)
    {
        foreach (TutorialCollider tutorialCollider in TutorialColliders)
        {
            if (colliderName.ToLower() == tutorialCollider.ColliderName.ToLower())
            {
                tutorialCollider.BaseObject.SetActive(true);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(ContinueTutorial) HudScript.SetContinueTutorial();
            if(ContinueDialogue) (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();

            BaseObject.SetActive(false);
        }
    }
}
