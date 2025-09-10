using UnityEngine;

public class AriesAnimationTriggers : MonoBehaviour
{
    public PushupScript PushupEventScript;

    public ParticleSystem PuffSystem;
    public void PerformPushup()
    {
        PushupEventScript.Pushup();
    }
    public void Puff()
    {
        PuffSystem.Play();
    }
}
