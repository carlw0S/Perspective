using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float acceleration = 10.0f;
    public float decceleration = 0.9f;
    public float turnAroundAcceleration = 20.0f;
    public float maxSpeed = 25.0f;

    private Rigidbody rb;
    private TrailRenderer trail;
    private float xSpeed;
    private bool quickStep = false;
    private float quickStepZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
    }

    void Update()
    {
        xSpeed = rb.velocity.x;

        if (Input.GetKeyDown(KeyCode.O) && !quickStep)
        {
            quickStep = true;
            quickStepZ = rb.position.z + 2;
        }
        else if (Input.GetKeyDown(KeyCode.P) && !quickStep)
        {
            quickStep = true;
            quickStepZ = rb.position.z - 2;
        }
        Debug.Log(quickStep);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed / 2)
            trail.enabled = true;
        else if (xSpeed == 0)
            trail.enabled = false;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D))
        {
            if (xSpeed < 0)
                rb.AddForce(Vector3.right * turnAroundAcceleration * (Mathf.Abs(xSpeed) + 1), ForceMode.Acceleration);    // Frenado en función de la velocidad
            else if (xSpeed < maxSpeed)
                rb.AddForce(Vector3.right * acceleration, ForceMode.Acceleration);
            else
            {
                Vector3 v = rb.velocity;
                v.x = maxSpeed;
                rb.velocity = v;
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A))
        {
            if (xSpeed > 0)
                rb.AddForce(Vector3.left * turnAroundAcceleration * (Mathf.Abs(xSpeed) + 1), ForceMode.Acceleration);
            else if (xSpeed > -maxSpeed)
                rb.AddForce(Vector3.left * acceleration, ForceMode.Acceleration);
            else
            {
                Vector3 v = rb.velocity;
                v.x = -maxSpeed;
                rb.velocity = v;
            }
        }
        else if (Mathf.Abs(xSpeed) > 0.01f)
        {
            Vector3 v = rb.velocity;
            v.x *= decceleration;
            rb.velocity = v;
        }
        else if (xSpeed != 0)
        {
            Vector3 v = rb.velocity;
            v.x = 0;
            rb.velocity = v;
        }

        QuickStep();
    }

    private void QuickStep()
    {
        if (quickStep)
        {
            if (Mathf.Abs(rb.position.z - quickStepZ) > 0.1f)
                rb.position = Vector3.Lerp(rb.position, new Vector3(rb.position.x, rb.position.y, quickStepZ), 0.25f);
            else
            {
                rb.position = new Vector3(rb.position.x, rb.position.y, quickStepZ);
                quickStep = false;
            }
        }
    }
}
