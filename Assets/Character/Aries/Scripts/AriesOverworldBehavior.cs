using System.Collections;
using UnityEngine;

public class AriesOverworldBehavior : OverworldBehavior
{
    public OverworldPositionScript OverworldController;
    private Animator thisAnimator;

    void Start()
    {
        thisAnimator = GetComponent<Animator>();
    }

    public IEnumerator Judgement()
    {
        yield return StartCoroutine(WalkToStation(0));
        thisAnimator.SetBool("Sitting", true);
        thisAnimator.SetBool("Looming", false);
        PrayerScript.instance.ActivateJudgement();
    }

    public IEnumerator GrabSoda()
    {
        yield return StartCoroutine(WalkToStation(5));
        thisAnimator.SetBool("LeftGrab", true);

        yield return StartCoroutine(WaitForAnimation(3));

        yield return StartCoroutine(WalkToStation(0));
        thisAnimator.SetBool("LeftGrab", false);

        yield return StartCoroutine(WaitForAnimation(2));
        thisAnimator.SetBool("Sitting", true);
    }

    public IEnumerator WalkToStation(int StationIdx)
    {
        if (OverworldController.CurrentStation == StationIdx) yield break;
        OverworldController.StartWalkTo(StationIdx);
        while(OverworldController.CurrentStation != StationIdx)
        {
            yield return null;
        }
    }

    public IEnumerator WaitForAnimation(int repeatWait)
    {
        AnimatorStateInfo stateInfo = thisAnimator.GetCurrentAnimatorStateInfo(0);
        int startingStateHash = stateInfo.shortNameHash;
        for (int i = 0; i < repeatWait; i++)
        {
            stateInfo = thisAnimator.GetCurrentAnimatorStateInfo(0);
            startingStateHash = stateInfo.shortNameHash;

            while (thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == startingStateHash)
            {
                yield return null;
            }
        }
        yield return null;
    }

    public override void ExecuteBehavior(string submitName, string behavior)
    {
        if (NameSource.SpeakerName.ToLower() != submitName.ToLower() && NameSource.NickName.ToLower() != submitName.ToLower()) return;

        PrayerScript.instance.DeactivateJudgement();

        if (behavior.ToLower() == "soda")
        {
            StartCoroutine(GrabSoda());
        }

        if (behavior.ToLower() == "judge")
        {
            StartCoroutine(Judgement());
        }
    }
}
