using UnityEngine;
using System.Collections;

public class CPPlayerController : BaseObject 
{
	[Header("References")]
    public CPPlayerStateManager StateManager;
    public SpriteRenderer PlayerSprite;
    public Animator PlayerAnimator;

	[Header("Walk Settings")]
    public float WalkSpeed = 1f;
    public float WalkStopDecay = 0.5f;
    public float StopThresholdSpeed = 0.01f;

	[Header("Run Settings")]
	public float RunSpeed = 1f;
	public float RunStopDecay = 0.5f;
	public float RunStopThresholdSpeed = 0.05f;

	[Header("Jump Settings")]
	public float JumpAcceleration = 5f;
	public float MaxJumpSpeed = 2f;
	public float MinJumpTime = 0.85f;
	public float LandingDelayTime = 1f;

	private const float kColliderSkinWidth = 0.01f;
    private Vector3 mVelocity = Vector3.zero;
    private bool mGunDrawn = false;

	private int mGroundLayerMask = -1;

	public enum FacingDirections
    {
        LEFT,
        RIGHT,
    }

    private FacingDirections mFacingDirection = FacingDirections.LEFT;

	// Use this for initialization
	protected override void Start () 
	{
        StateManager.DependencySetup(this);
	}
	
	// Update is called once per frame
	private void Update () 
	{
		GatherInput();
        ProcessState();
        MoveTransform();

		if (mGunDrawn) 
		{
			//TODO: This is just here to silence a warning that this variable isn't used yet.
		}
	}

    private void ProcessState()
    {
        switch (StateManager.PlayerState)
        {
            case CPPlayerStateManager.PlayerStates.IDLE:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_COLLIDE:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_HURT:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_JUMP:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_DRAW_GUN:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_ATTACK:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_SHOOT:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_RELOAD:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_CROUCH:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_CROUCH_COLLIDE:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_CROUCH_HURT:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_CROUCH_ATTACK:
                break;
            case CPPlayerStateManager.PlayerStates.IDLE_CROUCH_SHOOT:
                break;
            case CPPlayerStateManager.PlayerStates.WALKING:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_TURN:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_BRAKING:

                mVelocity *= WalkStopDecay;

                if (mVelocity.magnitude <= StopThresholdSpeed)
                {
                    mVelocity = Vector3.zero;
                    StateManager.SetState(CPPlayerStateManager.PlayerStates.IDLE, this);					
                }

                break;
            case CPPlayerStateManager.PlayerStates.WALK_COLLIDE:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_HURT:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_JUMP:

				if (!mRigidbody.isKinematic)
				{
					mRigidbody.isKinematic = true;
				}

				mRigidbody.AddForce(Vector3.up * (JumpAcceleration * Time.deltaTime), ForceMode.Impulse);

				if (StateManager.TimeInState > MinJumpTime && !Input.GetKey(KeyCode.Space))
				{
					StateManager.SetState(CPPlayerStateManager.PlayerStates.WALK_JUMP_FALL, this);
				}

                break;
            case CPPlayerStateManager.PlayerStates.WALK_JUMP_FALL:

				if (mRigidbody.isKinematic)
				{
					mRigidbody.isKinematic = false;
				}

				if (CheckIfGrounded())
				{
					StateManager.SetState(CPPlayerStateManager.PlayerStates.IDLE, this);
				}
				
                break;
            case CPPlayerStateManager.PlayerStates.WALK_JUMP_LAND:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_DRAW_GUN:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_ATTACK:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_SHOOT:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_RELOAD:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_CROUCH:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_CROUCH_COLLIDE:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_CROUCH_HURT:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_CROUCH_ATTACK:
                break;
            case CPPlayerStateManager.PlayerStates.WALK_CROUCH_SHOOT:
                break;
            case CPPlayerStateManager.PlayerStates.ROLL:
                break;
            case CPPlayerStateManager.PlayerStates.ROLL_COLLIDE:
                break;
            case CPPlayerStateManager.PlayerStates.ROLL_HURT:
                break;
            case CPPlayerStateManager.PlayerStates.RUNNING:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_TURN:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_BRAKING:

				mVelocity *= RunStopDecay;
				
				if (mVelocity.magnitude <= RunStopThresholdSpeed)
				{
					mVelocity = Vector3.zero;
					StateManager.SetState(CPPlayerStateManager.PlayerStates.IDLE, this);					
				}

			break;
		case CPPlayerStateManager.PlayerStates.RUN_COLLIDE:
			break;
            case CPPlayerStateManager.PlayerStates.RUN_HURT:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_JUMP:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_JUMP_FALL:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_JUMP_LAND:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_DRAW_GUN:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_ATTACK:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_SHOOT:
                break;
            case CPPlayerStateManager.PlayerStates.RUN_RELOAD:
                break;
            default:
            break;
        }
    }

    private void GatherInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            mVelocity = Vector3.zero;
            mVelocity += Input.GetKey(KeyCode.LeftShift) ? Vector3.left * RunSpeed : Vector3.left * WalkSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            mVelocity = Vector3.zero;
			mVelocity += Input.GetKey(KeyCode.LeftShift) ? Vector3.right * RunSpeed : Vector3.right * WalkSpeed;
		}
	}

    private void MoveTransform()
    {
        mTransform.position += mVelocity * Time.deltaTime;
    }

    private void OnGUI()
    {
        if (Input.GetKey (KeyCode.A) || Input.GetKey(KeyCode.D)) 
        {
            if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.IDLE
                || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALKING
                || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALK_BRAKING
			    || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.RUNNING
			    || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.RUN_BRAKING)
			{
				if (!Input.GetKey(KeyCode.LeftShift))
				{ 
					if (StateManager.SetState(CPPlayerStateManager.PlayerStates.WALKING, this))
	                {
	                    PlayerAnimator.SetTrigger("GoToWalk");
						FlipSprite(Input.GetKey(KeyCode.A) ? FacingDirections.LEFT : FacingDirections.RIGHT);
	                }	
				}
				else
				{
					if (StateManager.SetState(CPPlayerStateManager.PlayerStates.RUNNING, this))
					{
						PlayerAnimator.SetTrigger("GoToRun");
						FlipSprite(Input.GetKey(KeyCode.A) ? FacingDirections.LEFT : FacingDirections.RIGHT);
					}
				}
			}
		}
        
        if (Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp(KeyCode.D)) 
        {
            if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALKING)
            {
                if (StateManager.SetState(CPPlayerStateManager.PlayerStates.WALK_BRAKING, this))
                {
                    PlayerAnimator.SetTrigger("GoToWalkBrake"); //TODO: Braking animation
                }
            }
			else if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.RUNNING)
			{
				if (StateManager.SetState(CPPlayerStateManager.PlayerStates.RUN_BRAKING, this))
				{
					PlayerAnimator.SetTrigger("GoToRunBrake"); //TODO: Braking animation
				}
			}
		}       

		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKey (KeyCode.Space)) 
		{
			if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.IDLE ||
			    StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALKING)
			{
				if(StateManager.SetState(CPPlayerStateManager.PlayerStates.WALK_JUMP, this))
				{
					//PlayerAnimator.SetTrigger("GoToJump");
				}
			}
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

	private bool CheckIfGrounded()
	{
		if (mGroundLayerMask == -1) 
		{
			mGroundLayerMask = 1 << LayerMask.NameToLayer("GroundLayer");
		}

		bool grounded = false;

		Ray ray = new Ray (PlayerSprite.transform.position, Vector3.down);
		RaycastHit hit = new RaycastHit ();

		if (Physics.Raycast(ray, out hit, (PlayerSprite.sprite.texture.height * 0.5f) + kColliderSkinWidth, mGroundLayerMask))
		{
			grounded = true;
		}

		return grounded;

	}
}
