using UnityEngine;
using System.Collections;

public class TestOrbitCamera : MonoBehaviour 
{
    public Transform PlayerTransform;
    public Transform OrbitPoint;
    public float PlayerFollowSpeed = 5f;
    public float OrbitXAccel = 2f;
    public float OrbitYAccel = 2f;
    public float OrbitDecay = 0.95f;
    public float OrbitStopThreshold = 0.01f;
    public float OrbitMaxSpeed = 2f;
    public float MaxPitch = 89f;
    public float MinPitch = 10f;
    public float ScrollSpeed = 2f;
    public float MinZoom = 10f;
    public float MaxZoom = 40f;

    private Vector3 mOrbitVelocty = Vector3.zero;
	
    private Transform mTransform;

	private void Start () 
    {
        mTransform = transform;
        Screen.lockCursor = true;
        Screen.showCursor = false;
	}
	
	
	private void Update () 
    {
        Vector3 lookTargetRotation = PlayerTransform.position - transform.position;

        mTransform.forward = Vector3.Lerp(mTransform.forward, lookTargetRotation, PlayerFollowSpeed * Time.deltaTime);

        float mouseX = (Input.GetAxis("MouseX") * OrbitXAccel) * Time.deltaTime;

        if (mouseX == 0)
        {
            mOrbitVelocty.x *= OrbitDecay;

            if (Mathf.Abs(mOrbitVelocty.x) <= OrbitStopThreshold)
            {
                mOrbitVelocty.x = 0;
            }
        } 
        else
        {
            mOrbitVelocty.x += Mathf.Clamp(mouseX, -OrbitMaxSpeed, OrbitMaxSpeed);
        }

//        float mouseY = (Input.GetAxis("MouseY") * OrbitYAccel) * Time.deltaTime;
//        
//        if (mouseY == 0)
//        {
//            mOrbitVelocty.y *= OrbitDecay;
//            
//            if (Mathf.Abs(mOrbitVelocty.y) <= OrbitStopThreshold)
//            {
//                mOrbitVelocty.y = 0;
//            }
//        } 
//        else
//        {
//            mOrbitVelocty.y += Mathf.Clamp(mouseY, -OrbitMaxSpeed, OrbitMaxSpeed);
//        }

        mTransform.RotateAround(OrbitPoint.position, Vector3.up, mOrbitVelocty.x * Time.deltaTime);
       // mTransform.RotateAround(OrbitPoint.position, Vector3.right, mOrbitVelocty.y * Time.deltaTime);

        Vector3 currentRot = mTransform.rotation.eulerAngles;

//        if (currentRot.x > MaxPitch)
//        {
//            currentRot.x = MaxPitch;
//            mTransform.rotation = Quaternion.Euler(currentRot);
//        }
//
//        if (currentRot.x < MinPitch)
//        {
//            currentRot.x = MinPitch;
//            mTransform.rotation = Quaternion.Euler(currentRot);
//        }

        Vector3 playerRotation = PlayerTransform.localRotation.eulerAngles;

        playerRotation.y = currentRot.y;

        PlayerTransform.localRotation = Quaternion.Euler(playerRotation);
	}
}
