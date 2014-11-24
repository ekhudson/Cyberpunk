﻿using UnityEngine;
using System.Collections;

public class OrbitScript : MonoBehaviour 
{
    public Transform target;
    public float distance = 10.0f;
    
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    
    private float x = 0.0f;
    private float y = 0.0f;
    
    
    private void  Start () 
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }
    
    private void  LateUpdate () 
    {
        if (target && Input.GetMouseButton(1)) 
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y += Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            
            y = ClampAngle(y, yMinLimit, yMaxLimit);
            
            var rotation = Quaternion.Euler(y, x, 0f);
            var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
            
            transform.rotation = rotation;
            transform.position = position;
        }
    }
    
    public static float ClampAngle (float angle, float min, float max) 
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp (angle, min, max);
    }
}

