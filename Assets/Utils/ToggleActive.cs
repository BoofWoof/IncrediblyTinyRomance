using UnityEngine;

public class ToggleActive : MonoBehaviour
{

    public bool StartActive = true;
    public delegate void ToggleActiveDelegate(bool state);
    public ToggleActiveDelegate toggleActiveDelegate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(StartActive);
    }

    public void ToggleState()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        toggleActiveDelegate.Invoke(gameObject.activeSelf);
    }
}
