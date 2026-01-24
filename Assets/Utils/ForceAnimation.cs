using System.Collections.Generic;
using UnityEngine;

public class ForceAnimation : MonoBehaviour
{
    public List<string> AnimationNames;

    public Animator targetAnimator;

    public void ActivateAnimation(float triggerIndexTemp)
    {
        int triggerIndex = (int)triggerIndexTemp;
        targetAnimator.Play(AnimationNames[triggerIndex], 0, 0f);
        targetAnimator.Update(0f);
    }
}
