using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CameraController))]
public class SimplePlayer : MonoBehaviour
{
    public float speed;

    public Controls controls = new Controls ();
    CameraController cameraController;

    void Start ()
    {
        cameraController = GetComponent<CameraController> ();
    }

    void Update ()
    {

        if (Input.GetKey (controls.forward))
        {
            cameraController.Move (-transform.forward, speed);
        }
        if (Input.GetKey (controls.backward))
        {
            cameraController.Move (transform.forward, speed);
        }
        if (Input.GetKey (controls.right))
        {
            cameraController.Move (transform.right, speed);
        }
        if (Input.GetKey (controls.left))
        {
            cameraController.Move (-transform.right, speed);
        }
    }
}
