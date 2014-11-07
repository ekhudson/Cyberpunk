using UnityEngine;
using System.Collections;

public class CoinScript : BaseObject 
{
    public enum CoinStates
    {
        IDLE,
        HIGHLIGHTED,
        GRABBED,
    }

    public Renderer HighlightRenderer;
    public Rigidbody CoinRigidbody;
    public float MaxVelocity;

    private CoinStates mCoinState = CoinStates.IDLE;

    private void Start()
    {
        HighlightRenderer.enabled = false;


    }

    private void FixedUpdate()
    {
        switch (mCoinState)
        {
            case CoinStates.IDLE:
            break;
                
            case CoinStates.HIGHLIGHTED:

            break;
                
            case CoinStates.GRABBED:

                //Vector3 forceVector = CoinUserInterfaceManager.MouseBoardPosition - mTransform.position;
                //CoinRigidbody.AddRelativeForce(forceVector, ForceMode.Acceleration);

            break;
        }

        if (mRigidbody.velocity.sqrMagnitude > MaxVelocity * MaxVelocity)
        {
            Vector3.ClampMagnitude(mRigidbody.velocity, MaxVelocity);
        }
    }

    private void OnDrawGizmos()
    {
//        Vector3 forceVector = CoinUserInterfaceManager.MouseBoardPosition - mTransform.position;
//
//        Gizmos.DrawLine(mTransform.position, mTransform.position + forceVector);
//
//        switch (mCoinState)
//        {
//            case CoinStates.IDLE:
//                break;
//                
//            case CoinStates.HIGHLIGHTED:
//                
//            break;
//                
//            case CoinStates.GRABBED:         
//
//            break;
//        }
    }

    private void OnMouseOver()
    {
        SetState(CoinStates.HIGHLIGHTED);
    }

    private void OnMouseEnter()
    {
        SetState(CoinStates.HIGHLIGHTED);
    }
	
    private void OnMouseExit()
    {
        SetState(CoinStates.IDLE);
    }

    private void OnMouseDrag()
    {
        SetState(CoinStates.GRABBED);
    }

    private void OnMouseUp()
    {
        if (mCoinState == CoinStates.GRABBED)
        {
            SetState(CoinStates.IDLE);
        }
    }

    private void SetState(CoinStates newState)
    {
        if (newState == mCoinState)
        {
            return;
        }

        switch (newState)
        {
            case CoinStates.IDLE:
                HighlightRenderer.enabled = false;
            break;

            case CoinStates.HIGHLIGHTED:
                HighlightRenderer.enabled = true;
            break;

            case CoinStates.GRABBED:
            break;
        }

        mCoinState = newState;
    }
}
