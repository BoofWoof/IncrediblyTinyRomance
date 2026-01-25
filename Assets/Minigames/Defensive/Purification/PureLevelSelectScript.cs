using UnityEngine;

public class PureLevelSelectScript : MonoBehaviour
{
    public void StartLevelPack(string packName)
    {
        if (PurificationHolderScript.LevelHolders.ContainsKey(packName.ToLower()))
        {
            PurificationHolderScript.LevelHolders[packName.ToLower()].LevelStart();
            OverworldBehavior.AriesBehavior("puff");
        }
    }
}
