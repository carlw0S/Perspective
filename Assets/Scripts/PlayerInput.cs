using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public int inputType = 1;
    public int bufferFrameDuration = 10;
    public float tiltThreshold = 0.1f;
    public float swipeMinSpeed = 5.0f;

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

    

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Application.targetFrameRate = 60;   // MOVE SOMEWHERE ELSE EVENTUALLY
            inputType = 2;
        }
    }

    void Update()
    {
        switch (inputType)
        {
            case 1:     KeyboardInput();    break;
            case 2:     MobileInput();      break;
            default:    KeyboardInput();    break;
        }

        Debug.Log(Vector2.SignedAngle(Vector2.right, Vector2.down));
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

    private void KeyboardInput()
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

    private void MobileInput()
    {
        Vector3 acceleration = Input.acceleration;
        
        forwardHold = (acceleration.x > tiltThreshold);
        backwardHold = (acceleration.x < -tiltThreshold);

        jump = false;
        quickStepLeft = false;
        quickStepRight = false;
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            float swipeSpeed = t.deltaPosition.magnitude / t.deltaTime;

            if (swipeSpeed >= swipeMinSpeed)
            {
                float swipeAngle = Vector2.SignedAngle(Vector2.right, t.deltaPosition);

                jump = (swipeAngle > 60 && swipeAngle < 120);
                quickStepLeft = (swipeAngle > 150 || swipeAngle < -150);
                quickStepRight = (swipeAngle > -30 && swipeAngle < 30);
            }
            jumpHold = (t.phase != TouchPhase.Ended);
        }
    }
}