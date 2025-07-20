using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppScript : MonoBehaviour
{
    public GameObject AppRoot;
    public GameObject PreviousApp;

    public bool HideOnStart = true;
    public bool Active = true;

    public delegate void HideApp();
    public event HideApp OnHideApp;
    public delegate void ShowApp();
    public event ShowApp OnShowApp;

    public void Start()
    {
        if (HideOnStart)
        {
            Hide(false);
            RegisterInputActions();
        }
    }

    public void RegisterInputActions()
    {
        InputManager.PlayerInputs.Phone.AppReturn.performed += context => Hide(true);
    }

    public void Swap(AppScript newApp)
    {
        newApp.Show(AppRoot);
        Hide(false);
    }

    public void Show(GameObject previousApp)
    {
        Active = true;
        if (previousApp != null)
        {
            PreviousApp = previousApp;
        }
        AppRoot.transform.localPosition = new Vector3(0, 0, 0);

        OnShowApp?.Invoke();
    }

    public void Hide(bool revealLast)
    {
        Active = false;
        if (PreviousApp != null && revealLast)
        {
            Debug.Log(gameObject.name);
            PreviousApp.GetComponent<AppScript>().Show(null);
            PreviousApp = null;
        }
        AppRoot.transform.position = new Vector3(0, -100, 0);

        OnHideApp?.Invoke();
    }
}
