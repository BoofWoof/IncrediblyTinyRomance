using UnityEngine;

[CreateAssetMenu(fileName = "PosterUnlock", menuName = "Upgrades/PosterUnlock")]
public class PosterUnlockSO : UpgradesAbstract
{
    [Header("Poster Name")]
    public string PosterName;

    public override void OnBuy()
    {
        UnlockablesManager.UnlockPortrait(PosterName);
    }
}
