using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MaterialGradient : MonoBehaviour
{
    public Image SourceImage;

    public string FloatName;
    public float StartingFloat;
    public float EndingFloat;
    public float TransitionPeriod;
    public float HoldEndValue;
    public UnityEvent OnCompletion;
    void Start()
    {
        StartCoroutine(DoGradient());
    }

    IEnumerator DoGradient()
    {
        float timePassed = 0;
        Debug.Log("a");
        while (timePassed < TransitionPeriod)
        {
            Debug.Log("b");
            timePassed += Time.deltaTime;
            SourceImage.materialForRendering.SetFloat(FloatName, Mathf.Lerp(StartingFloat, EndingFloat, timePassed / TransitionPeriod));
            yield return null;
        }
        Debug.Log("c");
        SourceImage.material.SetFloat(FloatName, EndingFloat);

        yield return new WaitForSeconds(HoldEndValue);
        OnCompletion.Invoke();
    }
}
