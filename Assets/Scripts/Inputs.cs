using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public int bufferFrameDuration = 10;

    public bool forward;
    public bool forwardHold;
    public bool backward;
    public bool backwardHold;
    public bool jump;
    public bool jumpHold;
    public bool quickStepLeft;
    public bool quickStepRight;



    private int bufferedInput;
    private int bufferFrameCounter;

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D))
        {
            forward = true;
            SetBufferedInput(1);
        }
        else if (bufferedInput != 1)
        {
            forward = false;
        }
        forwardHold = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D);



        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A))
        {
            backward = true;
            SetBufferedInput(2);
        }
        else if (bufferedInput != 2)
        {
            backward = false;
        }
        backwardHold = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A);



        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
            SetBufferedInput(3);
        }
        else if (bufferedInput != 3)
        {
            jump = false;
        }
        jumpHold = Input.GetKey(KeyCode.Space);



        if (Input.GetKeyDown(KeyCode.O))
        {
            quickStepLeft = true;
            SetBufferedInput(4);
        }
        else if (bufferedInput != 4)
        {
            quickStepLeft = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            quickStepRight = true;
            SetBufferedInput(5);
        }
        else if (bufferedInput != 5)
        {
            quickStepRight = false;
        }
    }

    void FixedUpdate()
    {
        if (bufferedInput != 0 && (bufferFrameCounter++) >= bufferFrameDuration)
        {
            CleanBuffer();
        }
    }



    public void CleanBuffer()       // To be called after a buffereable input (mainly the not-hold ones) has been used
    {
        bufferedInput = 0;
    }



    private void SetBufferedInput(int x)
    {
        bufferedInput = x;
        bufferFrameCounter = 0;
    }
}