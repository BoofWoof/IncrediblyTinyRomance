using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

public class AriesOverworldBehavior : OverworldBehavior
{
    public OverworldPositionScript OverworldController;
    private Animator thisAnimator;

    [Header("AttackTapInfo")]
    public VoiceLineSO AttackTapAriesVoice;
    public VoiceLineSO AttackTapMiloVoice;

    public LookScript AriesLook;

    void Start()
    {
        thisAnimator = GetComponent<Animator>();
    }

    public IEnumerator AttackTap()
    {
        yield return StartCoroutine(WalkToStation(0));
        thisAnimator.SetBool("Sitting", true);
        thisAnimator.SetBool("Looming", false);
        thisAnimator.SetBool("SitForward", true);

        yield return StartCoroutine(WaitForAnimation("SitForwardIdle"));

        thisAnimator.SetTrigger("AttackTap");

        yield return new WaitForSeconds(1f);

        MusicSelectorScript.SetOverworldSong(5);
        CharacterSpeechScript.BroadcastSpeechAttempt("A", AttackTapAriesVoice);

        float waitTime = 19f - 3.8f;
        yield return new WaitForSeconds(waitTime);

        CharacterSpeechScript.BroadcastSpeechAttempt("M", AttackTapMiloVoice);

        yield return new WaitForSeconds(24f - waitTime);
        CrossfadeScript.PauseMusic();
    }

    public IEnumerator PuffAttack(float wait)
    {
        if (wait >= 0) yield return WaitForDialogueToEnd();

        GameStateMonitor.DangerActive = true;

        yield return StartCoroutine(WalkToStation(0));
        thisAnimator.SetBool("Sitting", true);
        thisAnimator.SetBool("Looming", false);
        thisAnimator.SetBool("SitForward", true);

        yield return StartCoroutine(WaitForAnimation("SitForwardIdle"));

        thisAnimator.SetTrigger("Puff");
    }

    public IEnumerator Judgement(float wait)
    {
        yield return new WaitForSeconds(wait);

        AriesLook.HeadLookWeight = 1;

        yield return StartCoroutine(WalkToStation(0));

        thisAnimator.SetBool("Sitting", true);
        thisAnimator.SetBool("Looming", false);
        thisAnimator.SetBool("SitForward", false);

        PrayerScript.instance.ActivateJudgement();
    }

    public IEnumerator PlaceCard(float wait)
    {
        yield return new WaitForSeconds(wait);

        AriesLook.HeadLookWeight = 1;

        yield return StartCoroutine(WalkToStation(0));

        thisAnimator.SetBool("Sitting", false);
        thisAnimator.SetBool("Looming", false);
        thisAnimator.SetBool("SitForward", false);

        yield return StartCoroutine(WaitForAnimation("StandingIdle"));

        thisAnimator.SetTrigger("SetDownCard");

        yield return StartCoroutine(WaitForAnimation("SetDownCard"));
        yield return StartCoroutine(WaitForAnimation("StandingIdle"));

        StartCoroutine(Judgement(0));
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

    public IEnumerator WaitForDialogueToEnd()
    {
        while (GameStateMonitor.isEventActive())
        {
            yield return null;
        }
    }

    public IEnumerator WaitForAnimation(string animationStateName)
    {
        while (!thisAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
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

    public override void ExecuteBehavior(string submitName, string behavior, float wait = 0f)
    {
        if (NameSource.SpeakerName.ToLower() != submitName.ToLower() && NameSource.NickName.ToLower() != submitName.ToLower()) return;

        PrayerScript.instance.DeactivateJudgement();

        if (behavior.ToLower() == "soda")
        {
            StartCoroutine(GrabSoda());
        }

        if (behavior.ToLower() == "judge")
        {
            StartCoroutine(Judgement(wait));
        }

        if (behavior.ToLower() == "puff")
        {
            StartCoroutine(PuffAttack(wait));
        }

        if (behavior.ToLower() == "tap")
        {
            StartCoroutine(AttackTap());
        }

        if (behavior.ToLower() == "card")
        {
            StartCoroutine(PlaceCard(wait));
        }
    }
}
