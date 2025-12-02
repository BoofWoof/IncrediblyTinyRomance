using System.Collections;
using UnityEngine;

public class BlendShapeTransitionScript : MonoBehaviour
{
    public SkinnedMeshRenderer targetSkinnedMesh;

    public float StartingValue;
    public float EndingValue;

    public float TransitionPeriod;

    public int ShapekeyIdx;

    public void StartTransition()
    {
        StartCoroutine(TransitionBlendshape());
    }

    public IEnumerator TransitionBlendshape()
    {
        float timePassed = 0f;

        while (timePassed < TransitionPeriod)
        {
            timePassed += Time.deltaTime;
            float newWeight = Mathf.Lerp(StartingValue, EndingValue, timePassed/TransitionPeriod);
            targetSkinnedMesh.SetBlendShapeWeight(ShapekeyIdx, newWeight);
            yield return null;
        }
        targetSkinnedMesh.SetBlendShapeWeight(ShapekeyIdx, EndingValue);
    }
}
