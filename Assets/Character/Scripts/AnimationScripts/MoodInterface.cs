using UnityEngine;

public abstract class MoodInterface : MonoBehaviour
{
    public float Anger = 0;
    public float AngerTarget = 0;
    public float AngerUpdateRate = 50f;

    public void Update()
    {
        UpdateAnger();
    }

    public virtual void UpdateAnger()
    {
        Anger = Mathf.MoveTowards(Anger, AngerTarget, Time.deltaTime * AngerUpdateRate);
    }

    public void SetAnger(float newAnger)
    {
        AngerTarget = newAnger;
    }

}
