using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;

    private Transform t;
    private Vector3 cameraOffset;



    void Start()
    {
        t = GetComponent<Transform>();
        cameraOffset = t.position - player.transform.position;
    }

    void Update()
    {
        t.position = player.transform.position + cameraOffset;
    }
}
