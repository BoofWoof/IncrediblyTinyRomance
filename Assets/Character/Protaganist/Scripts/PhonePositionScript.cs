using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PhonePositionScript : MonoBehaviour
{
    [Header("Objects")]
    public Transform phone;
    public GameObject screen;

    [Header("Tuning")]
    public float movement_speed = 0.1f;
    public float rotation_speed = 180f;

    public Transform raised_transfom;
    public Transform lowered_transform;
    public Vector3 scale = new Vector3(0.03622f, 0.03622f, -0.061263f);

    [HideInInspector]
    public bool moving = false;
    private bool raised = false;

    PlayerControls input;

    public delegate void PhoneStateCallback(bool raised);
    static public event PhoneStateCallback PhoneToggled;

    private void Awake()
    {
        input = new PlayerControls();
    }
    private void OnEnable()
    {
        input.Enable();
        input.Overworld.TogglePhone.performed += TogglePhone;
    }
    private void OnDisable()
    {
        input.Disable();
        input.Overworld.TogglePhone.performed -= TogglePhone;
    }

    private void TogglePhone(InputAction.CallbackContext ctx)
    {
        if (raised) StartCoroutine(LowerPhone());
        else StartCoroutine(RaisePhone());
    }
    public IEnumerator RaisePhone()
    {
        if (moving)
        {
            yield break;
        }
        PhoneToggled.Invoke(true);
        moving = true;
        raised = true;
        while (phone.localPosition != raised_transfom.localPosition || phone.localRotation != raised_transfom.localRotation)
        {
            phone.localRotation = Quaternion.RotateTowards(phone.localRotation, raised_transfom.localRotation, rotation_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localRotation.eulerAngles, raised_transfom.localRotation.eulerAngles) < 0.01f)
            {
                phone.localRotation = raised_transfom.localRotation;
            }
            phone.localPosition = Vector3.MoveTowards(phone.localPosition, raised_transfom.localPosition, movement_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localPosition, raised_transfom.localPosition) < 0.001f)
            {
                phone.localPosition = raised_transfom.localPosition;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.2f);
        moving = false;
    }

    public IEnumerator LowerPhone()
    {
        if (moving)
        {
            yield break;
        }
        PhoneToggled.Invoke(false);
        moving = true;
        raised = false;
        yield return new WaitForSeconds(0.2f);
        while (phone.localPosition != lowered_transform.localPosition || phone.localRotation != lowered_transform.localRotation)
        {
            phone.localRotation = Quaternion.RotateTowards(phone.localRotation, lowered_transform.localRotation, rotation_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localRotation.eulerAngles, lowered_transform.localRotation.eulerAngles) < 0.01f)
            {
                phone.localRotation = lowered_transform.localRotation;
            }
            phone.localPosition = Vector3.MoveTowards(phone.localPosition, lowered_transform.localPosition, movement_speed * Time.deltaTime);
            if (Vector3.Distance(phone.localPosition, lowered_transform.localPosition) < 0.001f)
            {
                phone.localPosition = lowered_transform.localPosition;
            }
            yield return new WaitForEndOfFrame();
        }
        moving = false;
    }
}
