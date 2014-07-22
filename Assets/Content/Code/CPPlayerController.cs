using UnityEngine;
using System.Collections;

public class CPPlayerController : BaseObject 
{
    public CPPlayerStateManager StateManager;
    public SpriteRenderer PlayerSprite;
    public Animator PlayerAnimator;

    public float WalkSpeed = 1f;
    public float WalkStopDecay = 0.5f;
    public float StopThresholdSpeed = 0.01f;

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
        ProcessState();
        MoveTransform();
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
                break;
            case CPPlayerStateManager.PlayerStates.WALK_JUMP_FALL:
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
            if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.IDLE
                || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALKING
                || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALK_BRAKING)
            {
                if (StateManager.SetState(CPPlayerStateManager.PlayerStates.WALKING, this))
                {
                    PlayerAnimator.SetTrigger("GoToWalk");
                }

                FlipSprite(FacingDirections.LEFT);
            }
        }
        
        if (Input.GetKeyUp (KeyCode.A)) 
        {
            if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALKING)
            {
                if (StateManager.SetState(CPPlayerStateManager.PlayerStates.WALK_BRAKING, this))
                {
                    PlayerAnimator.SetTrigger("GoToIdle"); //TODO: Braking animation
                }
            }
        }

        if (Input.GetKeyDown (KeyCode.D)) 
        {
            if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.IDLE
                || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALKING
                || StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALK_BRAKING)
            {
                if (StateManager.SetState(CPPlayerStateManager.PlayerStates.WALKING, this))
                {
                    PlayerAnimator.SetTrigger("GoToWalk");
                }
            }

            FlipSprite(FacingDirections.RIGHT);
        }
        
        if (Input.GetKeyUp (KeyCode.D)) 
        {
            if (StateManager.PlayerState == CPPlayerStateManager.PlayerStates.WALKING)
            {
                if (StateManager.SetState(CPPlayerStateManager.PlayerStates.WALK_BRAKING, this))
                {
                    PlayerAnimator.SetTrigger("GoToIdle");
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
}
