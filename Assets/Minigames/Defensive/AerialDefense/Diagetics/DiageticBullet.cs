using UnityEngine;

public class DiageticBullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.up * 400f * transform.lossyScale.x;
    }
}
