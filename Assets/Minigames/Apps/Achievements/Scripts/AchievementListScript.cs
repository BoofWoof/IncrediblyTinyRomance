using PixelCrushers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static ArchiveScript;

public struct PriorityAchievement {
    public int Priority;
    public AchievementAbstractSO Achievement;
}
public class AchievementListScript : Saver
{
    public GameObject ContentTarget;
    public GameObject AchievementPrefab;

    public List<PriorityAchievement> SortedAchievementList = new List<PriorityAchievement>();

    public static List<string> CompletedAchievementNames = new List<string>();
    public static List<string> ActivatedAchievementNames = new List<string>();

    public GameObject EmptyQueueText;

    public ParticleSystem AchievementParticles;

    public AudioSource UnlockSound;

    public static AchievementListScript instance;

    public Sprite NotificationSprite;

    #region SAVE
    [Serializable]
    public class ArchievementSaveData
    {
        public List<string> CompletedAchievementSaveData;
    }

    public override string RecordData()
    {
        ArchievementSaveData newSaveData = new ArchievementSaveData()
        {
            CompletedAchievementSaveData = CompletedAchievementNames
        };
        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        CompletedAchievementNames = SaveSystem.Deserialize<ArchievementSaveData>(s).CompletedAchievementSaveData;
        UpdateList();
    }
    #endregion

    override public void Awake()
    {
        base.Awake();

        instance = this;
        CompletedAchievementNames = new List<string>();
        ActivatedAchievementNames = new List<string>();
    }

    override public void OnEnable()
    {
        base.OnEnable();
        PhonePositionScript.PhoneToggled += OnPhoneRaise;
    }

    override public void OnDisable()
    {
        base.OnDisable();
        PhonePositionScript.PhoneToggled -= OnPhoneRaise;
    }

    public void OnPhoneRaise(bool raised)
    {
        UpdateList();
    }

    public void UpdateList()
    {
        ClearList();
        foreach (PriorityAchievement pAchievement in SortedAchievementList)
        {
            string title = pAchievement.Achievement.Title;
            if (CompletedAchievementNames.Contains(title))
            {
                if (!ActivatedAchievementNames.Contains(title))
                {
                    AddFinishedAchievement(title);
                    ActiveBroadcast.BroadcastActivation(pAchievement.Achievement.ActivationData);
                }

                continue;
            }

            GameObject newAchievementObject = Instantiate(AchievementPrefab);
            newAchievementObject.GetComponent<AchievementMenuItemScript>().AssignAchievementData(pAchievement.Achievement);
            newAchievementObject.transform.parent = ContentTarget.transform;
            newAchievementObject.transform.localRotation = Quaternion.identity;
            newAchievementObject.transform.localScale = Vector3.one;
            newAchievementObject.transform.localPosition = Vector3.zero;
        }
        CheckIfListIsEmpty();
    }

    public static void CheckIfListIsEmpty()
    {
        instance.EmptyQueueText.SetActive(instance.ContentTarget.transform.childCount == 0);
    }
    public void ClearList()
    {
        foreach (Transform child in ContentTarget.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public static void AddAchievementsStatic(List<AchievementAbstractSO> achievements, int priority)
    {
        instance.AddAchievements(AddPriotiyToAchievements(achievements, priority));
    }

    private void AddAchievements(List<PriorityAchievement> achievements)
    {
        int insertIdx = 0;

        foreach (PriorityAchievement pAchievement in SortedAchievementList)
        {
            if (pAchievement.Priority > achievements[0].Priority) break;
            insertIdx++;
        }
        SortedAchievementList.InsertRange(insertIdx, achievements);

        UpdateList();
    }

    private static List<PriorityAchievement> AddPriotiyToAchievements(List<AchievementAbstractSO> achievements, int priority)
    {
        List<PriorityAchievement> newPAchievementList = new List<PriorityAchievement>();

        foreach (AchievementAbstractSO achievement in achievements)
        {
            newPAchievementList.Add(new PriorityAchievement{
                Achievement = achievement,
                Priority = priority
                });
        }

        return newPAchievementList;
    }

    public static void AddFinishedAchievement(string AchievementName)
    {
        if (!CompletedAchievementNames.Contains(AchievementName))
        {
            CompletedAchievementNames.Add(AchievementName);
            instance.UnlockSound.Play();
            instance.AchievementParticles.Play();
        }
        if(!ActivatedAchievementNames.Contains(AchievementName)) ActivatedAchievementNames.Add(AchievementName);
    }

    public void ShowNotification()
    {
        NotificationMenuScript.SetNotification("Achievement", NotificationSprite);
    }

    public void HideNotification()
    {
        NotificationMenuScript.ReleaseNotification("Achievement");
    }
}
