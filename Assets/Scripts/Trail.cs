using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public float trailEnablingSpeed = 20.0f;


    private Rigidbody rb;
    private TrailRenderer trail;
    private float rbSpeedMagnitude;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        rbSpeedMagnitude = rb.velocity.magnitude;
        EnableTrail();
    }



    private void EnableTrail()
    {
        if (rbSpeedMagnitude >= trailEnablingSpeed)
        {
            trail.enabled = true;
        }
        else if (rbSpeedMagnitude == 0)
        {
            trail.enabled = false;
        }
    }
}
