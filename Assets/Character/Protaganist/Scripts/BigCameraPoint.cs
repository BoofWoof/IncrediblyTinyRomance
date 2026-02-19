using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BigCameraPoint : MonoBehaviour
{
    public float MaxActivationDistance = 5f;
    private GameObject TargetActivationObject = null;

    private static int _QuestionsAvailable;
    public static int QuestionsAvailable {
        get
        {
            return _QuestionsAvailable;
        }
        set
        {
            _QuestionsAvailable = value;
            instance.OnQuestionsAvailable?.Invoke(value > 0);
        }
    }

    public UnityEvent<bool> OnQuestionsAvailable;

    public static BigCameraPoint instance;

    public void OnEnable()
    {
        instance = this;
        Lua.RegisterFunction("AddQuestion", this, SymbolExtensions.GetMethodInfo(() => AddQuestion()));
    }

    public void ActivateObjects(InputAction.CallbackContext context)
    {
        if (!context.started) return; 
        if (!PlayerCam.EnableCameraMovement || CursorStateControl.MenuUp || Cursor.lockState == CursorLockMode.Confined) return;
        if (TargetActivationObject == null) return;
        ActivatableObjectScript aos = TargetActivationObject.GetComponent<ActivatableObjectScript>();
        if (aos != null)
        {
            aos.Activate();
            QuestionsAvailable--;
        }
    }

    public void AddQuestion()
    {
        QuestionsAvailable++;
    }

    public void Clear()
    {
        if (TargetActivationObject != null)
        {
            TargetActivationObject = null;
            ReticleScript.instance.SetDefault();
        }
    }

    // Update is called once per frame
    public void ScanForTarget()
    {
        if(QuestionsAvailable <= 0)
        {
            if (TargetActivationObject != null)
            {
                TargetActivationObject = null;
                ReticleScript.instance.SetDefault();
            }
            return;
        }

        // Create a ray from the center of the screen
        Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, MaxActivationDistance))
        {
            ActivatableObjectScript aos = hit.collider.gameObject.GetComponent<ActivatableObjectScript>();
            if (aos != null)
            {
                if (ReticleScript.instance.Inspecting) return;
                if (TargetActivationObject != null)
                {
                    ReticleScript.instance.SetDefault();
                }
                TargetActivationObject = hit.collider.gameObject;
                if (aos.ObjectEnabled)
                {
                    ReticleScript.instance.SetQuestion();
                }
            }
            else
            {
                if (TargetActivationObject != null)
                {
                    TargetActivationObject = null;
                    ReticleScript.instance.SetDefault();
                }
            }
        }
        else
        {
            if (TargetActivationObject != null)
            {
                TargetActivationObject = null;
                ReticleScript.instance.SetDefault();
            }
        }
    }
}
