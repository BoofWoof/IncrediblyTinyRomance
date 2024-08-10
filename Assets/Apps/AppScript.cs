using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppScript : MonoBehaviour
{
    public GameObject AppRoot;
    public GameObject PreviousApp;

    private PlayerControls inputs;

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



public void Show(GameObject previousApp)
    {
        PreviousApp = previousApp;
        AppRoot.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void Hide(bool revealLast)
    {
        if (PreviousApp != null && revealLast) PreviousApp.GetComponent<AppScript>().Show(gameObject);
        AppRoot.transform.position = new Vector3(0, -100, 0);
    }
}
