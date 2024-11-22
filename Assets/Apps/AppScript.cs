using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppScript : MonoBehaviour
{
    public GameObject AppRoot;
    public GameObject PreviousApp;

    private PlayerControls inputs;

    public bool HideOnStart = true;

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
        inputs = new PlayerControls();
        inputs.Phone.AppReturn.performed += context => Hide(true);
        PhonePositionScript.PhoneToggled += PhoneToggle;
    }
    private void OnEnable()
    {
        if (inputs != null) inputs.Enable();
    }

    private void OnDisable()
    {
        if (inputs != null) inputs.Disable();
    }

    private void OnDestroy()
    {
        PhonePositionScript.PhoneToggled -= PhoneToggle;
    }

    private void PhoneToggle(bool raised)
    {
        if (raised)
        {
            inputs.Enable();
        }
        else
        {
            inputs.Disable();
        }
    }

    public void Swap(AppScript newApp)
    {
        newApp.Show(AppRoot);
        Hide(false);
    }

    public void Show(GameObject previousApp)
    {
        if (previousApp != null)
        {
            PreviousApp = previousApp;
        }
        AppRoot.transform.localPosition = new Vector3(0, 0, 0);

        if (inputs != null) inputs.Enable();

        OnShowApp?.Invoke();
    }

    public void Hide(bool revealLast)
    {
        if (PreviousApp != null && revealLast)
        {
            Debug.Log(gameObject.name);
            PreviousApp.GetComponent<AppScript>().Show(null);
            PreviousApp = null;
        }
        AppRoot.transform.position = new Vector3(0, -100, 0);

        if (inputs != null) inputs.Disable();

        OnHideApp?.Invoke();
    }
}
