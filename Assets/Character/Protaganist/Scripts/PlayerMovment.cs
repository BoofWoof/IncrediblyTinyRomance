using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    Vector2 MovementInput = Vector2.zero;

    [Header("Audio")]
    public AudioSource WalkingSounds;

    private float SpeedMultiplier = 1f;

    private void Awake()
    {
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        InputManager.PlayerInputs.Overworld.Move.performed += context => MovementInput = context.ReadValue<Vector2>();
        InputManager.PlayerInputs.Overworld.Move.canceled += context => MovementInput = Vector2.zero;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();

        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    public void SetSpeed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SpeedMultiplier = 0.3f;
        }
        if (context.canceled)
        {
            SpeedMultiplier = 1f;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = MovementInput.x;
        verticalInput = MovementInput.y;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * Mathf.Sqrt(transform.lossyScale.x) * SpeedMultiplier, ForceMode.Force);
        if(rb.linearVelocity.magnitude > 0.1f)
        {
            if(!WalkingSounds.isPlaying) WalkingSounds.Play();
        } else
        {
            if (WalkingSounds.isPlaying) WalkingSounds.Stop();
        }
        if (SpeedMultiplier < 0.9f) WalkingSounds.Stop();
    }
}
