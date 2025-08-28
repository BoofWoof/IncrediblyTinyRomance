using System.Collections;
using UnityEngine;

public class AriesOverworldBehavior : MonoBehaviour
{
    public OverworldPositionScript OverworldController;
    private Animator thisAnimator;

    void Start()
    {
        thisAnimator = GetComponent<Animator>();

        StartCoroutine(GrabSoda());
    }

    public IEnumerator GrabSoda()
    {
        yield return StartCoroutine(WalkToStation(5));
        thisAnimator.SetBool("LeftGrab", true);

        yield return StartCoroutine(WaitForAnimation());

        Debug.Log("AAAA");
        yield return StartCoroutine(WalkToStation(0));
        thisAnimator.SetBool("LeftGrab", false);
    }

    public IEnumerator WalkToStation(int StationIdx)
    {
        OverworldController.StartWalkTo(StationIdx);
        while(OverworldController.CurrentNode != StationIdx)
        {
            yield return null;
        }
    }

    public IEnumerator WaitForAnimation()
    {
        AnimatorStateInfo stateInfo = thisAnimator.GetCurrentAnimatorStateInfo(0);
        int startingStateHash = stateInfo.shortNameHash;

        while (thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == startingStateHash)
        {
            yield return null;
        }

        stateInfo = thisAnimator.GetCurrentAnimatorStateInfo(0);
        startingStateHash = stateInfo.shortNameHash;

        while (thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == startingStateHash)
        {
            yield return null;
        }

        stateInfo = thisAnimator.GetCurrentAnimatorStateInfo(0);
        startingStateHash = stateInfo.shortNameHash;

        while (thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == startingStateHash)
        {
            yield return null;
        }

        yield return null;
    }
}
