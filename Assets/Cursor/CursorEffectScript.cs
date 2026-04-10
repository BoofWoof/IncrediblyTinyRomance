using UnityEngine;
using UnityEngine.InputSystem;

public class CursorEffectScript : MonoBehaviour
{
    public RectTransform TargetCanvas;
    public Camera TargetCamera;

    public GameObject TapParticles;

    public GameObject MouseMovementParticles;

    private Vector2 localPoint;

    private ParticleSystem.EmissionModule em;

    public void Start()
    {
        em = MouseMovementParticles.GetComponentInChildren<ParticleSystem>().emission;
        em.rateOverDistance = 0f;
    }

    public void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            TargetCanvas,
            mousePos,
            TargetCamera,
            out localPoint);

        MouseMovementParticles.transform.localPosition = localPoint;
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        Debug.Log("Triggered");
        if (ctx.phase == InputActionPhase.Started)
        {
            MouseMovementParticles.transform.localPosition = localPoint;

            em.rateOverDistance = 0.5f;
        }
        if (ctx.phase == InputActionPhase.Canceled)
        {
            TapParticles.transform.localPosition = localPoint;

            TapParticles.GetComponentInChildren<ParticleSystem>().Play();

            em.rateOverDistance = 0f;
        }
    }
}
