using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputActionEvent : MonoBehaviour
{
    [Header("Input Binding")]
    public InputActionReference inputActionRef;

    [Header("Action")]
    public UnityEvent onInputPerformed;

    private InputAction inputAction;

    private void OnEnable()
    {
        if (inputActionRef != null)
        {
            inputAction = inputActionRef.action;
            inputAction.Enable();
            inputAction.performed += OnInputPerformedHandler;
        }
    }

    private void OnDisable()
    {
        if (inputAction != null)
        {
            inputAction.performed -= OnInputPerformedHandler;
            inputAction.Disable();
        }
    }

    private void OnInputPerformedHandler(InputAction.CallbackContext context)
    {
        onInputPerformed?.Invoke();
    }
}
