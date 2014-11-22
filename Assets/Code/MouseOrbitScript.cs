﻿using UnityEngine;
using System.Collections;

public class MouseOrbitScript : BaseObject 
{
    public Transform Target;
    public float distance = 10.0f;
    public float FollowDistance = 5.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private float x = 0.0f;
    private float y = 0.0f;

    private CameraStates mCameraState = CameraStates.IDLE;
    private Transform mCurrentTarget;
    private float mCurrentDistace = 10.0f;
    private Transform mSecondFollowPoint = null;

	public enum CameraStates
	{
        IDLE,
        FOLLOWING_COIN,
	}

    public CameraStates GetState
    {
        get
        {
            return mCameraState;
        }
    }

    public Transform SecondaryFollowPoint
    {
        get
        {
            return mSecondFollowPoint;
        }
    }

    private void  Start () 
    {
        mCurrentTarget = Target;
        mCurrentDistace = distance;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        base.Start();

    }

    private void  LateUpdate () 
    {
        Quaternion rotation = mTransform.rotation;
        Vector3 position = mTransform.position;

        if (mCameraState == CameraStates.IDLE)
        {

            if ( (mCurrentTarget && Input.GetMouseButton(1)))            
            {
                x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
                y += Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
                
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                
                rotation = Quaternion.Euler(y, x, 0f);
                //position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
            }
        }
        else if (mCameraState == CameraStates.FOLLOWING_COIN)
        {

        }

        position = rotation * new Vector3(0.0f, 0.0f, -mCurrentDistace) + mCurrentTarget.position;
        transform.rotation = rotation;
        transform.position = position;

        if (mCameraState == CameraStates.FOLLOWING_COIN && mSecondFollowPoint != null)
        {

            mTransform.rotation = Quaternion.RotateTowards(mTransform.rotation, Quaternion.LookRotation( (mSecondFollowPoint.position - transform.position).normalized ), 2f);
            //mTransform.LookAt(mSecondFollowPoint.position);
        }
    }

    public void SetSecondaryFollowPoint(Transform transform)
    {
        mSecondFollowPoint = transform;
    }

    public void NewSecondFollowPoint(Vector3 position)
    {
        if (mSecondFollowPoint != null)
        {
            GameObject.Destroy(mSecondFollowPoint);
        }

        mSecondFollowPoint = new GameObject("Follow Point").transform;
        mSecondFollowPoint.position = position;
    }

    public void SetState(CameraStates newState)
    {
        if (mCameraState == newState)
        {
            return;
        }

        if (newState == CameraStates.IDLE)
        {
            mCurrentDistace = distance;
            mCurrentTarget = Target;
            mSecondFollowPoint = null;
        }
        else if (newState == CameraStates.FOLLOWING_COIN)
        {
            mCurrentDistace = FollowDistance;
        }

        mCameraState = newState;
    }

    public void SetTarget(Transform newTarget)
    {
        if (mCurrentTarget == newTarget)
        {
            return;
        }

        mCurrentTarget = newTarget;
    }

    public static float ClampAngle (float angle, float min, float max) {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp (angle, min, max);
    }
}
