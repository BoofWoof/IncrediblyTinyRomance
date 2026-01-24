using UnityEngine;

public class SetObjectScaleScript : MonoBehaviour
{
    public GameObject TargetObject;
    

    public void SetObjectScale(float newScale)
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAA");
        Debug.Log("Player scale set to " + newScale.ToString());
        TargetObject.transform.localScale = Vector3.one * newScale;
    }
}
