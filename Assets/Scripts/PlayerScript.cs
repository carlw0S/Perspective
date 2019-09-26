using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Stats")]
    public float acceleration = 10.0f;
    public float decceleration = 0.9f;
    public float turnAroundAcceleration = 20.0f;
    public float maxSpeed = 25.0f;
    public float lowSpeedThreshold = 0.001f;
    public float quickStepDistance = 2.0f;
    public float quickStepLerpInterpolant = 0.25f;
    public float quickStepSnapThreshold = 0.01f;
    public float trailActivation = 0.75f;       // Percentage of maxSpeed that marks when the trail will show.
    public float baseJumpSpeed = 6.5f;
    public float jumpReleaseDecceleration = 0.75f;
    public int shortJumpFrameDuration = 4;
    private int jumpFrameCounter = 0;

    private Rigidbody rb;
    private TrailRenderer trail;
    private Vector3 speed;              // Current speed.
    private float xSpeed,               // Current horizontal speed.
                  ySpeed;               // Current vertical speed.
    private bool quickStep = false;     // Indicates that the player is performing a quickstep.
    private float quickStepZ;           // Destination in the Z axis of the current quickstep.
    private bool jumping = false;
    private bool onGround = true;
    private Vector3 initialPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
        initialPos = rb.position;
    }

    void Update()
    {
        speed = rb.velocity;
        xSpeed = speed.x;
        ySpeed = speed.y;

        JumpInput();
        QuickStepInput();

        Trail();

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset
            rb.velocity = Vector3.zero;
            rb.position = initialPos;
        }
    }

    void FixedUpdate()
    {
        Run();
        Jump();
        if (quickStep)
            QuickStep();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            Vector3 v = speed;
            v.y = 0;
            rb.velocity = v;

            onGround = true;
            jumping = false;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }



    private void Run()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D))
        {
            if (xSpeed < 0)     // Turn around
                rb.AddForce(Vector3.right * turnAroundAcceleration * (Mathf.Abs(xSpeed) + 1), ForceMode.Acceleration);    // The more speed, the stronger the brake
            else if (xSpeed < maxSpeed)     // Move forward
                rb.AddForce(Vector3.right * acceleration, ForceMode.Acceleration);
            else    // Speed cap
            {
                Vector3 v = speed;
                v.x = maxSpeed;
                rb.velocity = v;
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A))
        {
            if (xSpeed > 0)     // Turn around
                rb.AddForce(Vector3.left * turnAroundAcceleration * (Mathf.Abs(xSpeed) + 1), ForceMode.Acceleration);
            else if (xSpeed > -maxSpeed)    // Move back
                rb.AddForce(Vector3.left * acceleration, ForceMode.Acceleration);
            else    // Speed cap
            {
                Vector3 v = speed;
                v.x = -maxSpeed;
                rb.velocity = v;
            }
        }
        else if (Mathf.Abs(xSpeed) > lowSpeedThreshold)     // Decceleration when no input
        {
            Vector3 v = speed;
            v.x *= decceleration;
            rb.velocity = v;
        }
        else if (xSpeed != 0)       // Manually set speeds lower than lowSpeedThreshold to 0 (otherwise, it keeps getting lower, but not null)
        {
            Vector3 v = speed;
            v.x = 0;
            rb.velocity = v;
        }
    }

    private void JumpInput()
    {
        jumpFrameCounter++;
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.AddForce(Vector3.up * baseJumpSpeed, ForceMode.VelocityChange);

            onGround = false;
            jumping = true;
            jumpFrameCounter = 0;
        }
    }

    private void Jump()
    {
        if (!Input.GetKey(KeyCode.Space) && jumping && speed.y > 0 && jumpFrameCounter >= shortJumpFrameDuration)    // The jump has a minimum duration of x frames
        {
            Vector3 v = speed;
            v.y *= jumpReleaseDecceleration;
            rb.velocity = v;
        }
    }

    private void QuickStepInput()
    {
        if (Input.GetKeyDown(KeyCode.O) && !quickStep)
        {
            quickStep = true;
            quickStepZ = rb.position.z + quickStepDistance;
        }
        else if (Input.GetKeyDown(KeyCode.P) && !quickStep)
        {
            quickStep = true;
            quickStepZ = rb.position.z - quickStepDistance;
        }
    }

    private void QuickStep()
    {
        if (Mathf.Abs(rb.position.z - quickStepZ) > quickStepSnapThreshold)
            rb.position = Vector3.Lerp(rb.position, new Vector3(rb.position.x, rb.position.y, quickStepZ), quickStepLerpInterpolant);
        else
        {   // When close to the destination of the quickstep, the player will snap into place
            rb.position = new Vector3(rb.position.x, rb.position.y, quickStepZ);
            quickStep = false;
        }
    }

    private void Trail()
    {
        if (Mathf.Abs(xSpeed) > (maxSpeed * trailActivation))
            trail.enabled = true;
        else if (xSpeed == 0)
            trail.enabled = false;
    }
}
