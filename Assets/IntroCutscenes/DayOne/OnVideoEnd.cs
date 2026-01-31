using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class OnVideoEnd : MonoBehaviour
{
    public UnityEvent OnVideoEndEvent;
    public VideoPlayer TargetVideoPlayer;

    public void Start()
    {
        TargetVideoPlayer.loopPointReached += VideoEndTrigger;
    }

    public void VideoEndTrigger(VideoPlayer vp)
    {
        Debug.Log("Triggering Video Over");
        OnVideoEndEvent?.Invoke();
    }
}
