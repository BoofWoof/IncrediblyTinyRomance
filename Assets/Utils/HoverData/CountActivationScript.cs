using UnityEngine;

public class CountActivationScript : MonoBehaviour
{
    public GameObject target;

    public int Count = 0;

    public void Start()
    {
        target.SetActive(false);
    }

    public void Increase()
    {
        Count++;
        CheckReveal();
    }

    public void Decrease()
    {
        Count--;
        CheckReveal();
    }

    public void CheckReveal()
    {
        target.SetActive(Count > 0);
    }
}
