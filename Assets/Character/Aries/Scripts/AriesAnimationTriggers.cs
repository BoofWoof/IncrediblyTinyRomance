using UnityEngine;

public class AriesAnimationTriggers : MonoBehaviour
{
    public PushupScript PushupEventScript;
    public void PerformPushup()
    {
        PushupEventScript.Pushup();
    }
}
