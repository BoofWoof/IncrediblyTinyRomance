using UnityEngine;

public class ADBlastScript : MonoBehaviour
{
    public float Speed = 20f;
    private RectTransform thisRect;
    public float LifeTime = 3f;

    public Rigidbody2D thisRB2D;

    public void Start()
    {
        thisRect = GetComponent<RectTransform>();

        Destroy(gameObject, LifeTime);

        thisRB2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        thisRB2D.linearVelocity = thisRect.up * Speed * transform.lossyScale.x;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("A");
        if (collision.gameObject.tag == "ADThreat")
        {
            AerialDefenseScript.ThreatDestroyed();

            Debug.Log("B");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
