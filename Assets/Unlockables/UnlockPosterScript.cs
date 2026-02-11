using UnityEngine;

public class UnlockPosterScript : MonoBehaviour
{
    public string PosterName;

    public void UnlockPoster()
    {
        UnlockablesManager.UnlockPortrait(PosterName);
    }
}
