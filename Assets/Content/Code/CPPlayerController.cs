using UnityEngine;
using System.Collections;

public class CPPlayerController : BaseObject 
{
	public SpriteRenderer PlayerSprite;
    public Animator PlayerAnimator;

    public float WalkSpeed = 1f;
    public float WalkStopDecay = 0.5f;

    private Vector3 mVelocity = Vector3.zero;
    private bool mGunDrawn = false;

	public enum FacingDirections
    {
        LEFT,
        RIGHT,
    }

    private FacingDirections mFacingDirection = FacingDirections.LEFT;

	// Use this for initialization
	protected override void Start () 
	{
			
	}
	
	// Update is called once per frame
	private void Update () 
	{
		GatherInput();
        MoveTransform();
	}

    private void GatherInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            mVelocity = Vector3.zero;
            mVelocity += Vector3.left * WalkSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            mVelocity = Vector3.zero;
            mVelocity += Vector3.right * WalkSpeed;
        }
    }

    private void MoveTransform()
    {
        mTransform.position += mVelocity * Time.deltaTime;
    }

    private void OnGUI()
    {
        if (Input.GetKeyDown (KeyCode.A)) 
        {
            PlayerAnimator.SetTrigger("GoToWalk");
            FlipSprite(FacingDirections.LEFT);
        }
        
        if (Input.GetKeyUp (KeyCode.A)) 
        {
            PlayerAnimator.SetTrigger("GoToIdle");
        }

        if (Input.GetKeyDown (KeyCode.D)) 
        {
            PlayerAnimator.SetTrigger("GoToWalk");
            FlipSprite(FacingDirections.RIGHT);
        }
        
        if (Input.GetKeyUp (KeyCode.D)) 
        {
            PlayerAnimator.SetTrigger("GoToIdle");
        }
    }

	private void FlipSprite(FacingDirections newFacingDirection)
	{
		if (newFacingDirection == mFacingDirection)
		{
            return;
		}

        switch (newFacingDirection)
        {
            case FacingDirections.LEFT:
            
            PlayerAnimator.transform.localScale = new Vector3(1f, 1f, 1f);

            break;

            case FacingDirections.RIGHT:
            
            PlayerAnimator.transform.localScale = new Vector3(-1f, 1f, 1f);
            
            break;
        }

        mFacingDirection = newFacingDirection;
	}
}
