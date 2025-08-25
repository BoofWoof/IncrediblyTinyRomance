using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FallingThreatScript : MonoBehaviour
{
    public float FadeInPeriod = 1.0f;
    public float FadeOutPeriod = 1.0f;
    public float MaintainPeriod = 0.5f;

    Coroutine FadeInAndOutCorouting;

    Image imageComponent;

    public Rigidbody2D thisRB2D;
    public float DropSpeed = 20f;

    public GameObject DestructionPingPrefab;
    public GameObject DetectionPingPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.color = new Color(1f, 1f, 1f, 0f);


        thisRB2D = GetComponent<Rigidbody2D>();
        thisRB2D.linearVelocity = Vector2.down * DropSpeed * transform.lossyScale.x;
    }

    public IEnumerator FadeInAndOut()
    {
        float timePassed = 0f;

        while (timePassed < FadeInPeriod)
        {
            timePassed += Time.deltaTime;
            imageComponent.color = new Color(1f, 1f, 1f, timePassed/FadeInPeriod);
            yield return null;
        }
        imageComponent.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(MaintainPeriod);

        timePassed = 0f;
        while (timePassed < FadeOutPeriod)
        {
            timePassed += Time.deltaTime;
            imageComponent.color = new Color(1f, 1f, 1f, 1 - timePassed / FadeOutPeriod);
            yield return null;
        }
        imageComponent.color = new Color(1f, 1f, 1f, 0f);
    }

    public void OnDestroy()
    {
        GameObject newPing = Instantiate(DestructionPingPrefab, transform.position, Quaternion.identity, transform.parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Scanner")
        {
            Instantiate(DetectionPingPrefab, transform.position, Quaternion.identity, transform.parent);
            StopAllCoroutines();
            FadeInAndOutCorouting = StartCoroutine(FadeInAndOut());
        }
        if (collision.gameObject.name == "DangerLine")
        {
            Destroy(gameObject);
            AerialDefenseScript.TakeDamage();
        }
    }
}
