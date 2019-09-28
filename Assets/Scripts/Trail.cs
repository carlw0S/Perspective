using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public float enablingSpeed = 25.0f;
    public float thickeningMultiplier = 2.0f;
    public float shrinkingMutiplier = 0.75f;
    public float minWidthMultiplier = 0.001f;
    public float maxWidthMultiplier = 1.0f;

    private Rigidbody rb;
    private TrailRenderer trail;
    private float rbSpeedMagnitude;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();

        trail.enabled = false;
        trail.widthMultiplier = minWidthMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        rbSpeedMagnitude = rb.velocity.magnitude;
        DrawTrail();
    }



    private void DrawTrail()
    {
        if (rbSpeedMagnitude >= enablingSpeed)
        {
            if (!trail.enabled)
            {
                trail.enabled = true;
            }

            float width = trail.widthMultiplier *= thickeningMultiplier;
            if (width < maxWidthMultiplier)
            {
                trail.widthMultiplier = width;
            }
            else
            {
                trail.widthMultiplier = maxWidthMultiplier;
            }
        }
        else if (trail.enabled)
        {
            if (trail.widthMultiplier <= minWidthMultiplier)
            {
                trail.enabled = false;
            }

            float width = trail.widthMultiplier *= shrinkingMutiplier;
            if (width > minWidthMultiplier)
            {
                trail.widthMultiplier = width;
            }
            else
            {
                trail.widthMultiplier = minWidthMultiplier;
            }
        }
    }
}
