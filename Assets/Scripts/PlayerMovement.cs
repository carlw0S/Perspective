using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* ------------------------------------------------ VARIABLES ------------------------------------------------ */

    [Header("Run Parameters")]
    public float runAcceleration = 10.0f;
    public float runDecceleration = 0.9f;
    public float airHorizontalDecceleration = 0.99f;
    public float maxRunSpeed = 25.0f;
    public float turnAroundAcceleration = 20.0f;
    public float stopThreshold = 0.1f;


    [Header("Jump Parameters")]
    public float baseJumpSpeed = 6.5f;
    public float jumpReleaseDecceleration = 0.75f;
    public int shortJumpFrameDuration = 4;
    private int jumpFrameCounter = 0;
    

    [Header("QuickStep Parameters")]
    public float quickStepDistance = 2.0f;
    public float quickStepLerpInterpolant = 0.25f;
    public float quickStepSnapThreshold = 0.01f;
    private float quickStepZdestination;


    [Header("Booleans")]
    public bool doJump = false;
    public bool jumping = false;
    public bool quickStepping = false;
    public bool airQuickStepped = false;



    [Header("Handy values")]    // These could be private
    public Vector3 speed;
    public float xSpeed;
    public float ySpeed;



    // Initial setup
    private Rigidbody rb;
    private Collisions coll;
    private Vector3 initialPos;
    private PlayerInput input;



    /* ------------------------------------------------ MONOBEHAVIOUR METHODS ------------------------------------------------ */

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collisions>();
        input = GetComponent<PlayerInput>();
        initialPos = rb.position;
    }

    void Update()
    {
        JumpInput();

        QuickStepInput(quickStepDistance);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    void FixedUpdate()
    {
        speed = rb.velocity;
        xSpeed = speed.x;       // EVERYTHING RELATED TO MOVEMENT SHOULD BE IN FIXEDUPDATE
        ySpeed = speed.y;       // (This was on Update before, and it caused some issues with jumping mainly)

        Run();
        Jump();
        QuickStep();
    }



    /* ------------------------------------------------ PRIVATE FUNCTIONS ------------------------------------------------ */

    private void Reset()
    {
        rb.velocity = Vector3.zero;
        rb.position = initialPos;
    }

    private void JumpInput()
    {
        if (input.jump && coll.onGround)
        {
            input.CleanBuffer();

            doJump = true;
        }
    }

    private void QuickStepInput(float dist)
    {
        if (input.quickStepLeft && !quickStepping && !airQuickStepped)
        {
            input.CleanBuffer();

            airQuickStepped = !coll.onGround;
            quickStepping = true;
            quickStepZdestination = rb.position.z + dist;
        }
        else if (input.quickStepRight && !quickStepping && !airQuickStepped)
        {
            input.CleanBuffer();

            airQuickStepped = !coll.onGround;
            quickStepping = true;
            quickStepZdestination = rb.position.z - dist;
        }
    }

    private void Jump()
    {
        if (jumpFrameCounter < shortJumpFrameDuration)
            jumpFrameCounter++;

        if (doJump)
        {
            doJump = false;
            jumpFrameCounter = 0;
            rb.AddForce(Vector3.up * baseJumpSpeed, ForceMode.VelocityChange);
            coll.onGround = false;      // I wish I didn't have to do this, but OnCollisionExit seems to be too slow...
            jumping = true;
        }
        // The jump has a minimum duration of "shortJumpFrameDuration" frames
        else if (!input.jumpHold && jumping && speed.y > 0 && jumpFrameCounter >= shortJumpFrameDuration)
        {
            Vector3 v = speed;
            v.y *= jumpReleaseDecceleration;
            rb.velocity = v;
        }
        else if (jumping && coll.onGround)
        {
            jumping = false;
        }
    }

    private void QuickStep()
    {
        if (quickStepping)
        {
            if (Mathf.Abs(rb.position.z - quickStepZdestination) > quickStepSnapThreshold)
            {   
                // Gradually step to a side
                rb.position = Vector3.Lerp(rb.position, new Vector3(rb.position.x, rb.position.y, quickStepZdestination), quickStepLerpInterpolant);
            }
            else
            {   
                // When close to the destination of the quickstep, the player will snap into place
                rb.position = new Vector3(rb.position.x, rb.position.y, quickStepZdestination);
                quickStepping = false;
            }
        }

        if (airQuickStepped && coll.onGround)
        {
            airQuickStepped = false;
        }
    }

    private void Run()
    {
        if (input.forwardHold)
        {
            if (xSpeed < 0)     // Turn around
            {
                rb.AddForce(Vector3.right * turnAroundAcceleration * (Mathf.Abs(xSpeed) + 1), ForceMode.Acceleration);
                // The higher the speed, the stronger the brake
            }
            else if (xSpeed < maxRunSpeed)     // Go forward
            {
                rb.AddForce(Vector3.right * runAcceleration, ForceMode.Acceleration);
            }
            else    // Speed cap
            {
                Vector3 v = speed;
                v.x = maxRunSpeed;
                rb.velocity = v;
            }
        }
        else if (input.backwardHold)
        {
            if (xSpeed > 0)     // Turn around
            {
                rb.AddForce(Vector3.left * turnAroundAcceleration * (Mathf.Abs(xSpeed) + 1), ForceMode.Acceleration);
            }
            else if (xSpeed > -maxRunSpeed)    // Go backward
            {
                rb.AddForce(Vector3.left * runAcceleration, ForceMode.Acceleration);
            }
            else    // Speed cap
            {
                Vector3 v = speed;
                v.x = -maxRunSpeed;
                rb.velocity = v;
            }
        }
        else if (Mathf.Abs(xSpeed) > stopThreshold)     // Decceleration when no input is detected
        {
            Vector3 v = speed;
            if (coll.onGround)
            {
                v.x *= runDecceleration;                // On the ground
            }
            else
            {
                v.x *= airHorizontalDecceleration;      // Airborne
            }
            rb.velocity = v;
        }
        else if (xSpeed != 0 && Mathf.Abs(xSpeed) < stopThreshold)       // Manually set speeds lower than "stopThreshold" to 0 (otherwise, it keeps getting lower, but not null)
        {
            Vector3 v = speed;
            v.x = 0;
            rb.velocity = v;
        }
    }
}
