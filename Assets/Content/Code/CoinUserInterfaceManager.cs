using UnityEngine;
using System.Collections;

public class CoinUserInterfaceManager : Singleton<CoinUserInterfaceManager> 
{

    public float MaxRaycastLength = 100f;

    private Ray mCurrentRay = new Ray();
    private RaycastHit mCurretRaycastHit;

    private Camera mCamera;
    private Transform mCameraTransform;

    private int mCoinLayermask = 1;

    private GameObject mCurrentCollidingObject = null;

	// Use this for initialization
	private void Start () 
    {
        mCamera = Camera.main;
        mCameraTransform = mCamera.transform;

        mCoinLayermask = 1 << LayerMask.NameToLayer("CoinLayer");
	}
	
	// Update is called once per frame
	private void OnGUI () 
    {
        CoinCast();

        if (mCurrentCollidingObject != null)
        {
            mCurrentCollidingObject.renderer.material.color = Color.cyan;
        }
	}

    private void CoinCast()
    {
        mCurrentRay.direction = mCameraTransform.forward;
        mCurrentRay.origin = mCameraTransform.position;

        if (Physics.Raycast(mCurrentRay, out mCurretRaycastHit, MaxRaycastLength, mCoinLayermask))
        {
            Debug.Log("Colliding with "+ mCurretRaycastHit.collider.gameObject.name); 
            mCurrentCollidingObject = mCurretRaycastHit.collider.gameObject;
        }
    }
}
