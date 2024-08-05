using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppScript : MonoBehaviour
{
    public GameObject AppRoot;
    public GameObject PreviousApp;
    public void Show(GameObject previousApp)
    {
        PreviousApp = previousApp;
        AppRoot.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void Hide()
    {
        if (PreviousApp != null) PreviousApp.GetComponent<AppScript>().Show(gameObject);
        AppRoot.transform.position = new Vector3(0, -10000, 0);
    }
}
