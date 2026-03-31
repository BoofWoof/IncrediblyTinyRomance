using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public bool DestroySelfOnDisable = false;
    public void DestroySelfMethod()
    {
        Destroy(gameObject);
    }

    public void OnDisable()
    {
        if(DestroySelfOnDisable) Destroy(gameObject);
    }
}
