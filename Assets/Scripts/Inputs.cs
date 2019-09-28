using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public bool forward;
    public bool forwardHold;
    public bool backward;
    public bool backwardHold;
    public bool jump;
    public bool jumpHold;
    public bool quickStepLeft;
    public bool quickStepRight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        forward = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D);
        forwardHold = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D);

        backward = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A);
        backwardHold = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A);

        jump = Input.GetKeyDown(KeyCode.Space);
        jumpHold = Input.GetKey(KeyCode.Space);

        quickStepLeft = Input.GetKeyDown(KeyCode.O);
        quickStepRight = Input.GetKeyDown(KeyCode.P);
    }
}