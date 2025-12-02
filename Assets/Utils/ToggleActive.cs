using UnityEngine;

public class ToggleActive : MonoBehaviour
{

    public bool StartActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(StartActive);
    }

    public void ToggleState()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
