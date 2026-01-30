using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementAbstractSO", menuName = "Scriptable Objects/AchievementAbstractSO")]
public abstract class AchievementAbstractSO : ScriptableObject
{
    public string Title;
    public string Objective;
    public string Flavor;
    public string ButtonText;

    public string OnBuyAnnouncement;

    public BroadcastStruct ActivationData;

    public abstract bool CheckCompletionCriteria();

}
