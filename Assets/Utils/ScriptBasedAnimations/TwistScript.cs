using UnityEngine;

public class TwistScript : MonoBehaviour
{
    public float TwistAmount;
    public float TwistSpeed;
    public void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, TwistAmount * Mathf.Sin(Time.time * TwistSpeed*2*Mathf.PI));
    }
}
