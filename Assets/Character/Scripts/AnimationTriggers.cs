using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    public void Impact(float Strength)
    {
        MoveCamera.moveCamera.ShakeScreen(1f, Strength);
    }
}
