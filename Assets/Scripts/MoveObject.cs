using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    public Transform cameraTransform;

    void Update()
    {
        float moveX = 0f; 
        float moveY = 0f; 
        float moveZ = 0f;   

        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;

        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        
        if (Input.GetKey(KeyCode.Z)) moveY = 1f;   // Move up
        if (Input.GetKey(KeyCode.X)) moveY = -1f; // Move down

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        Vector3 cameraUp = cameraTransform.up;

        cameraForward.y = 0f;
        cameraForward.Normalize();
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraRight * moveX + cameraForward * moveZ + cameraUp * moveY).normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        float rotate = 0f;
        if(Input.GetKey(KeyCode.Q)) rotate = 1f;
        if(Input.GetKey(KeyCode.E)) rotate = -1f;

        transform.Rotate(Vector3.up, rotate*rotationSpeed*Time.deltaTime, Space.World);
    }
}

