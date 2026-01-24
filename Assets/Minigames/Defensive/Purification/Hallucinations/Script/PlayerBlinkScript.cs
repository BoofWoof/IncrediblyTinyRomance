using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBlinkScript : MonoBehaviour
{
    public static PlayerBlinkScript instance;

    public GameObject TopLid;
    public GameObject BottomLid;

    public float TransitionPeriod;

    private Vector3 TopLidStartPosition;
    private Vector3 BottomLidStartPosition;

    public List<BroadcastStruct> OnBlinkEvent;

    public void OnEnable()
    {
        instance = this;

        TopLidStartPosition = TopLid.transform.localPosition;
        BottomLidStartPosition = BottomLid.transform.localPosition;
    }

    public static void StartBlink(List<BroadcastStruct> blinkEvents)
    {
        instance.OnBlinkEvent = blinkEvents;
        instance.StartCoroutine(instance.BlinkCoroutine());
    }

    public IEnumerator BlinkCoroutine()
    {
        TopLid.transform.localPosition = TopLidStartPosition;
        BottomLid.transform.localPosition = BottomLidStartPosition;

        float timePassed = 0;
        while (timePassed < TransitionPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / TransitionPeriod;
            TopLid.transform.localPosition = new Vector3 (0, Mathf.Lerp(TopLidStartPosition.y, 0, progress), 0);
            BottomLid.transform.localPosition = new Vector3(0, Mathf.Lerp(BottomLidStartPosition.y, 0, progress), 0);
            yield return null;
        }

        TopLid.transform.localPosition = new Vector3(0, 0, 0);
        BottomLid.transform.localPosition = new Vector3(0, 0, 0);

        foreach(BroadcastStruct blinkEvent in OnBlinkEvent)
        {
            ActiveBroadcast.BroadcastActivation(blinkEvent);
        }
        yield return null;

        timePassed = 0;
        while (timePassed < TransitionPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / TransitionPeriod;
            TopLid.transform.localPosition = new Vector3(0, Mathf.Lerp(0, TopLidStartPosition.y, progress), 0);
            BottomLid.transform.localPosition = new Vector3(0, Mathf.Lerp(0, BottomLidStartPosition.y, progress), 0);
            yield return null;
        }
        TopLid.transform.localPosition = TopLidStartPosition;
        BottomLid.transform.localPosition = BottomLidStartPosition;
    }

}
