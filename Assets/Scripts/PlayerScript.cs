using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float acceleration = 10.0f;
    public float decceleration = 0.9f;
    public float turnAroundAcceleration = 20.0f;
    public float maxSpeed = 25.0f;
    public float quickStepDistance = 2.0f;
    public float quickStepLerpInterpolant = 0.25f;
    public float trailActivation = 0.75f;       // Percentage of maxSpeed that marks when the trail will show.

    private Rigidbody rb;
    private TrailRenderer trail;
    private float xSpeed;               // Current horizontal speed.
    private bool quickStep = false;     // Indicates that the player is performing a quickstep.
    private float quickStepZ;           // Destination in the Z axis of the current quickstep.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
    }

    void Update()
    {
        xSpeed = rb.velocity.x;

        QuickStepInput();

        Trail();
    }

    void FixedUpdate()
    {
        Movement();
        Jump();
        if (quickStep)
            QuickStep();
    }



    private void Movement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D))
        {
            if (xSpeed < 0)     // Turn around
                rb.AddForce(Vector3.right * turnAroundAcceleration * (Mathf.Abs(xSpeed) + 1), ForceMode.Acceleration);    // The more speed, the stronger the brake
            else if (xSpeed < maxSpeed)     // Move forward
                rb.AddForce(Vector3.right * acceleration, ForceMode.Acceleration);
            else    // Speed cap
            {
                Vector3 v = rb.velocity;
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
                Vector3 v = rb.velocity;
                v.x = -maxSpeed;
                rb.velocity = v;
            }
        }
        else if (Mathf.Abs(xSpeed) > 0.01f)     // Decceleration when no input
        {
            Vector3 v = rb.velocity;
            v.x *= decceleration;
            rb.velocity = v;
        }
        else if (xSpeed != 0)       // Manually set speeds lower than 0.01 to 0 (it keeps getting lower but not null otherwise)
        {
            Vector3 v = rb.velocity;
            v.x = 0;
            rb.velocity = v;
        }
    }

    private void Jump()
    {
        
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
        if (Mathf.Abs(rb.position.z - quickStepZ) > 0.1f)
            rb.position = Vector3.Lerp(rb.position, new Vector3(rb.position.x, rb.position.y, quickStepZ), quickStepLerpInterpolant);
        else
        {   // When close to the destination of the quickstep, the player will snap into place
            rb.position = new Vector3(rb.position.x, rb.position.y, quickStepZ);
            quickStep = false;
        }
    }

    private void Trail()
    {
        if (Mathf.Abs(rb.velocity.x) > (maxSpeed * trailActivation))
            trail.enabled = true;
        else if (xSpeed == 0)
            trail.enabled = false;
    }
}
