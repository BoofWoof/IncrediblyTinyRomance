using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        if(moveDirection.magnitude > 0)
        {
            if(!WalkingSounds.isPlaying) WalkingSounds.Play();
        } else
        {
            if (WalkingSounds.isPlaying) WalkingSounds.Stop();
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.y);

        if(flatVel.magnitude > moveSpeed){
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = limitedVel;
        }
    }
}
