using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayEventScript : MonoBehaviour
{
    public float delay = 1f;
    public UnityEvent onDelayComplete;

    private Coroutine delayCoroutine;

    public void Trigger()
    {
        delayCoroutine = StartCoroutine(ExecuteAfterDelay());
    }

    public void ClearTrigger()
    {
        if(delayCoroutine != null) StopCoroutine(delayCoroutine);
    }

    private IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        onDelayComplete.Invoke();
    }
}
