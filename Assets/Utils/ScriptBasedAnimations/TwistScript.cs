using UnityEngine;

public class TwistScript : MonoBehaviour
{
    private Vector3 _StartPosition;

    public float TwistAmount;
    public float TwistSpeed;

    public Vector3 BounceAmount;
    public float BounceSpeed;

    public void Awake()
    {
        _StartPosition = transform.localPosition;
    }

    public void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, TwistAmount * Mathf.Sin(Time.time * TwistSpeed*2*Mathf.PI));
        transform.localPosition = _StartPosition + BounceAmount * Mathf.Sin(Time.time * BounceSpeed * 2 * Mathf.PI);
    }
}
