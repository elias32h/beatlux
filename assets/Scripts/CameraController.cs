﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraMover))]
public class CameraController : MonoBehaviour {
    public float speed = 10f;
    public float mouseSensitivity = 3f;
   

    private CameraMover move;

    void Start()
    {
        
        move = GetComponent<CameraMover>();
    }

    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");


        float zMov = Input.GetAxisRaw("Vertical");

        //Prevent user from moving forward (DELETE LATER!!!)
        /**if (Input.GetKey(KeyCode.W))
        {
            zMov = 0;
        }**/


        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;
        move.Move(velocity);

        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");


        Vector3 rotation = new Vector3(-xRot, yRot, 0f) * mouseSensitivity;
        //Vector3 camRotation = new Vector3(xRot, 0f, 0f) * mouseSensitivity;

        

        move.Rotate(rotation);
        //move.CamRotate(camRotation);


    }
}
