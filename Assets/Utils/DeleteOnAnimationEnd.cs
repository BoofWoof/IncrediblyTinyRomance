using UnityEngine;

public class DeleteOnAnimationEnd : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("DestroyAfterAnimation requires an Animator on " + gameObject.name);
            return;
        }

        // Get the length of the current animation
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        // Destroy the GameObject after the animation length
        Destroy(gameObject, animationLength);
    }
}
