using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrawlerPlayerComponentMecanim : BrawlerHittable
{
	private int mPlayerID = -1; //should get set from 0 - 4. If this is ever -1 it means this is not a valid player
	private int mAssociatedGamepad = -1; //again, if this is ever -1 it means no gamepad is assigned;
	private Color mPlayerColor = Color.white;
	private bool mIsActivePlayer = true;
	private bool mAIPlayer = false;

	//TODO: Wrap this in a PlayerAttributes class
	#region PlayerAttributes
	public float PlayerMinStrength = 10f;
	public float PlayerMaxStrength = 50f;
	public float AttackTime = 0.4f;
	public float AttackChargeTime = 1f;
	public float MinimumTimeBetweenAttacks = 0.5f;
	public float MoveSpeed = 1.0f;
	public float ClimbSpeed = 1.0f;
	public float JumpForce = 2.0f;
	public float MinimumJumpTime = 0.5f;
	public AnimationCurve JumpCurve = new AnimationCurve();
	public float DropForce = 2f;
	public float LandingTime = 0.2f;
	public float HurtTime = 0.3f;
	public float AirControl = 0.9f;
	public float ConstantFriction =  0.9f;
	public TriggerVolume PunchBox;
	public Transform HitParticle;
	#endregion

	#region Sprites
//	public Sprite DefaultSprite;
//	public Sprite JumpSprite;
//	public Sprite AttackSprite;
//	public Sprite JumpAttackSprite;
//	public Sprite LandSprite;
//	public Sprite MoveSprite;
//	public Sprite HurtSprite;
	#endregion

	protected Vector3 mTarget = Vector3.zero;
	protected CharacterEntity mController;
	
	private bool mIsDropping = false;  
	
	private Vector3 mInitialRotation;
	//private SpriteRenderer //mSpriteRenderer;
	private Animator mAnimator;
	private float mLastAttackEndTime;
	private Vector3 mLastMovingDirection;

	private Vector3 mCurrentAttackDirection;
	
	public enum PlayerStates
	{
		IDLE,
		MOVING,
		JUMPING,
		FALLING,
		LANDING,
		FROZEN,
		ATTACKING_GROUND,
		ATTACKING_AIR,
		JUMPING_JOYSTICK,
		HURT,
		ATTACKING_GROUND_CHARGING,
		ATTACKING_AIR_CHARGING,
	}
	
	protected PlayerStates mPlayerState = PlayerStates.IDLE;
	protected float mTimeInState = 0.0f;

	public int PlayerID
	{
		get
		{
			return mPlayerID;
		}
	}

	public int AssociatedGamepad
	{
		get
		{
			return mAssociatedGamepad;
		}
	}

	public Color PlayerColor
	{
		get
		{
			return mPlayerColor;
		}
	}

	public bool IsActivePlayer
	{
		get
		{
			return mIsActivePlayer;
		}
		set
		{
			mIsActivePlayer = value;
		}
	}

	public PlayerStates GetState
	{
		get
		{
			return mPlayerState;
		}
	}
	
	public CharacterEntity GetEntity
	{
		get
		{
			return mController;
		}
	} 

	public bool IsAI
	{
		get
		{
			return mAIPlayer;
		}
		set
		{
			mAIPlayer = value;
		}
	}
	
	protected override void Start()
	{
		base.Start ();
		EventManager.Instance.AddHandler<UserInputKeyEvent>(InputHandler);
		mController = GetComponent<CharacterEntity>();
		mAnimator = GetComponent<Animator>();
		//mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
		rigidbody.WakeUp();  

		foreach(BrawlerPlayerComponent player in BrawlerPlayerManager.Instance.PlayerList)
		{
			if (player.gameObject.activeSelf)
			{
				mPlayerID = player.PlayerID;
			}
		}

	}  
	
	protected void Update()
	{
		if (mController == null || mPlayerState == PlayerStates.FROZEN)
		{
			return;
		}
		
		switch(GetState)
		{
		case PlayerStates.IDLE:

			if (Mathf.Abs(mTarget.x) > 0f)
			{
				SetState(PlayerStates.MOVING);
			}

			if (!mController.IsGrounded)
			{
				SetState(PlayerStates.FALLING);
			}
			
			break;
			
		case PlayerStates.MOVING:

			if (Mathf.Abs(mTarget.x) <= 0f)
			{
				SetState(PlayerStates.IDLE);
			}

			if (!mController.IsGrounded)
			{
				SetState(PlayerStates.FALLING);
			}
			
			break;

		case PlayerStates.JUMPING_JOYSTICK:
		case PlayerStates.JUMPING:
			
			if (mTimeInState > JumpCurve.keys[JumpCurve.length - 1].time && mTimeInState > MinimumJumpTime)
			{
				SetState(PlayerStates.FALLING);
				return;
			}
			
			mController.SetGrounded(false);
			mController.Move( mController.BaseTransform.up * (JumpForce * JumpCurve.Evaluate(mTimeInState)));	

			mLastMovingDirection = mTarget;

			mTarget *= AirControl;
			
			break;
			
		case PlayerStates.FALLING:
			
			if (mController.IsGrounded)
			{
				SetState(PlayerStates.LANDING);
			}

			mLastMovingDirection = mTarget;

			mTarget *= AirControl;

			if (mIsDropping)
			{
				mTarget += Physics.gravity * DropForce;
			}
			
			break;
			
		case PlayerStates.LANDING:

			if (mTimeInState > LandingTime)
			{
				SetState(PlayerStates.IDLE);
			}
			
			break;
			
		case PlayerStates.ATTACKING_GROUND:

			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.IDLE);
				mLastAttackEndTime = Time.realtimeSinceStartup;
			}
			
			break;
		
		case PlayerStates.ATTACKING_AIR:
			
			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.FALLING);
				mLastAttackEndTime = Time.realtimeSinceStartup;
			}

			if (mController.IsGrounded)
			{
				SetState(PlayerStates.ATTACKING_GROUND, true);
				return;
			}
			
			break;
			
		case PlayerStates.HURT:

			if (mTimeInState > HurtTime)
			{
				SetState(PlayerStates.IDLE);
			}
			
			break;

		case PlayerStates.ATTACKING_GROUND_CHARGING:

			//mSpriteRenderer.color = Color.Lerp(//mSpriteRenderer.color, Color.white, mTimeInState);

			//if (mTimeInState > AttackChargeTime)
			//{
				//SetState(PlayerStates.ATTACKING_GROUND);
				//return;
			//}

			break;

		case PlayerStates.ATTACKING_AIR_CHARGING:

			//mSpriteRenderer.color = Color.Lerp(//mSpriteRenderer.color, Color.white, mTimeInState);

			//if (mTimeInState > AttackChargeTime)
			//{
				//SetState(PlayerStates.ATTACKING_AIR);
				//return;
			//}

			if (mController.IsGrounded)
			{
				SetState(PlayerStates.ATTACKING_GROUND_CHARGING, true);
				return;
			}

			mTarget += mLastMovingDirection;

			break;

		}		
		
		Vector3 norm = mTarget.normalized;
		//mController.Move( ((new Vector3(norm.x, 0, norm.z) * (MoveSpeed)) + new Vector3(0, mTarget.y, 0)) * Time.deltaTime);
		mTransform.position += ((new Vector3(norm.x, 0, norm.z) * (MoveSpeed)) + new Vector3(0, mTarget.y, 0)) * Time.deltaTime;
		mTarget = Vector3.zero; 
		
		if (ConstantFriction > 0)
		{
			Vector3 currentVelocity = mController.BaseRigidbody.velocity;
			mController.BaseRigidbody.velocity = new Vector3(currentVelocity.x * ConstantFriction, currentVelocity.y, currentVelocity.z * ConstantFriction);
		}
		
		mTimeInState += Time.deltaTime;
	}	
	
	public void MoveEntity(Vector3 direction)
	{
		mTarget += direction;
	}      

	public void SetState(PlayerStates state)
	{
		SetState(state, false);
	}

	public void SetState(PlayerStates state, bool carryOverTimeInState)
	{
		if (state == mPlayerState)
		{
			return;
		}

		if (state != PlayerStates.MOVING && mPlayerState == PlayerStates.MOVING)
		{
			mAnimator.SetBool("Walking", false);
		}
		
		switch(state)
		{
		case PlayerStates.IDLE:
			
			rigidbody.useGravity = true;
			//mSpriteRenderer.sprite = DefaultSprite;
			mIsDropping = false;
			
			break;
			
		case PlayerStates.MOVING:
			
			rigidbody.useGravity = true;
			//mSpriteRenderer.sprite = MoveSprite;
			mIsDropping = false;

			mAnimator.SetBool("Walking", true);
			
			break;

		case PlayerStates.JUMPING_JOYSTICK:

			if (mPlayerState == PlayerStates.FALLING)
			{
				return;
			}
			
			mController.SetGrounded(false);
			//mSpriteRenderer.sprite = JumpSprite;

			break;

		case PlayerStates.JUMPING:
			
			if (mPlayerState == PlayerStates.FALLING)
			{
				return;
			}
			
			mController.SetGrounded(false);
			//mSpriteRenderer.sprite = JumpSprite;
			
			break;
			
		case PlayerStates.FALLING:
			
			rigidbody.useGravity = true;
			//mSpriteRenderer.sprite = JumpSprite;
			
			break;
			
		case PlayerStates.LANDING:

			//mSpriteRenderer.sprite = LandSprite;
			
			break;

		case PlayerStates.ATTACKING_GROUND:

			//mSpriteRenderer.color = mPlayerColor;
			//mSpriteRenderer.sprite = AttackSprite;
			Attack(Mathf.Lerp(PlayerMinStrength, PlayerMaxStrength, Mathf.Clamp(0, 1, mTimeInState)) , 1, mCurrentAttackDirection);
			
			break;

		case PlayerStates.ATTACKING_AIR:
			
			//mSpriteRenderer.color = mPlayerColor;
			//mSpriteRenderer.sprite = JumpAttackSprite;
			Attack(Mathf.Lerp(PlayerMinStrength, PlayerMaxStrength, Mathf.Clamp(0, 1, mTimeInState)) , 1, mCurrentAttackDirection);
			
			break;

		case PlayerStates.HURT:

			//mSpriteRenderer.sprite = HurtSprite;
			mIsDropping = false;
			
			break;

		case PlayerStates.ATTACKING_AIR_CHARGING:

			//mSpriteRenderer.sprite = JumpSprite;

			break;

		case PlayerStates.ATTACKING_GROUND_CHARGING:

			//mSpriteRenderer.sprite = DefaultSprite;

			break;
		}	


		if (!carryOverTimeInState)
		{
			mTimeInState = 0.0f;
		}
		
		mPlayerState = state;
	}
	
	public void InputHandler(object sender, UserInputKeyEvent evt)
	{
		if (!gameObject.activeSelf)
		{
			return;
		}

		bool walking = false;

		if (evt.KeyBind != BrawlerUserInput.Instance.MoveCharacter)
		{
			//Debug.Log (string.Format("player {2} getting {0} for player {1}", evt.KeyBind.BindingName, (evt.PlayerIndexInt + 1).ToString(), mPlayerID.ToString()));
		}
		
//		if(evt.PlayerIndexInt != mPlayerID - 1 && evt.PlayerIndexInt != -1)
//		{
//			return;
//		}

		//Debug.Log("Doing");

		if (GetState == PlayerStates.IDLE || GetState == PlayerStates.MOVING || 
		    mPlayerState == PlayerStates.FALLING || mPlayerState == PlayerStates.JUMPING ||
		    mPlayerState == PlayerStates.ATTACKING_AIR || mPlayerState == PlayerStates.JUMPING_JOYSTICK ||
		    mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
		{
			
			if(evt.KeyBind == BrawlerUserInput.Instance.MoveLeft && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
				{
					return;
				}

				mTarget += -Camera.main.transform.right;

				walking = true;


//				if (mSpriteRenderer.transform.rotation.eulerAngles.y == 0)
//				{
//					mSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
//				}

			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.MoveRight && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
				{
					return;
				}

				mTarget += Camera.main.transform.right;

				walking = true;

//				if (mSpriteRenderer.transform.rotation.eulerAngles.y != 0)
//				{
//					mSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
//				}
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.MoveUp && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				
			}

			if(evt.KeyBind == BrawlerUserInput.Instance.MoveDown && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				if (mPlayerState == PlayerStates.FALLING && !mIsDropping)
				{
					mIsDropping = true;
				}
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.Jump)
			{
				if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
				{
					return;
				}

				switch(evt.Type)
				{
				case UserInputKeyEvent.TYPE.KEYDOWN: 
					
					SetState(PlayerStates.JUMPING);
					
					break;
					
				case UserInputKeyEvent.TYPE.KEYUP:
					
					SetState(PlayerStates.FALLING);
					
					break;
					
				case UserInputKeyEvent.TYPE.KEYHELD:

					if (mPlayerState == PlayerStates.IDLE || mPlayerState == PlayerStates.MOVING)
					{
						SetState(PlayerStates.JUMPING);
					}
					
					break;
					
				case UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN: 
					
					switch(mPlayerState)
					{
					case PlayerStates.IDLE:
						
						SetState(PlayerStates.JUMPING);
						
						break;
						
					case PlayerStates.MOVING:
						
						SetState(PlayerStates.JUMPING);
						
						break;
						
					case PlayerStates.JUMPING:
						
						break;
						
					case PlayerStates.FALLING:						
						
						
						break;
						
					case PlayerStates.LANDING:
						
						break;
						
					case PlayerStates.HURT:
						
						break;
					}
					
					break;
					
				case UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_UP:
					
					if (mPlayerState == PlayerStates.JUMPING)
					{
						SetState(PlayerStates.FALLING);
					}
					
					break;
					
				case UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_HELD:

					if (mPlayerState == PlayerStates.IDLE || mPlayerState == PlayerStates.MOVING)
					{
						SetState(PlayerStates.JUMPING);
					}
					
					break;
				}
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.PrimaryFire && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN))
			{
				
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.UseKey01 && evt.Type == UserInputKeyEvent.TYPE.KEYDOWN)
			{
				
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.UseKey02 && evt.Type == UserInputKeyEvent.TYPE.KEYDOWN)
			{
				
			}

			if (evt.KeyBind == BrawlerUserInput.Instance.Attack && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN))
			{
				if ( (Time.realtimeSinceStartup - mLastAttackEndTime) < MinimumTimeBetweenAttacks )
				{
					return;
				}

				if (mPlayerState == PlayerStates.IDLE || mPlayerState == PlayerStates.MOVING)
				{
					SetState(PlayerStates.ATTACKING_GROUND_CHARGING);
				}
				else if (mPlayerState == PlayerStates.JUMPING || mPlayerState == PlayerStates.JUMPING_JOYSTICK || mPlayerState == PlayerStates.FALLING)
				{
					SetState(PlayerStates.ATTACKING_AIR_CHARGING);
				}
				else if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
				{
				}
			}
			
			if (evt.KeyBind == BrawlerUserInput.Instance.MoveCharacter)
			{

				if (mPlayerState != PlayerStates.ATTACKING_AIR_CHARGING && mPlayerState != PlayerStates.ATTACKING_GROUND_CHARGING)
				{
					mTarget.x += (evt.JoystickInfo.AmountX);			

					if (evt.JoystickInfo.AmountX < 0 && mTransform.rotation.eulerAngles.y <= 90)
					{
						mTransform.rotation = Quaternion.Euler(new Vector3(0,270,0));
					}
					else if (evt.JoystickInfo.AmountX > 0 && mTransform.rotation.eulerAngles.y > 90)
					{
						mTransform.rotation = Quaternion.Euler(new Vector3(0,90,0));
					}

					if (evt.JoystickInfo.AmountX != 0)
					{
						walking = true;
					}


				}
				else
				{
					mCurrentAttackDirection = new Vector3(evt.JoystickInfo.AmountX, evt.JoystickInfo.AmountY, 0).normalized;
				}

				if (evt.JoystickInfo.AmountY > 0)
				{
					switch(mPlayerState)
					{
					case PlayerStates.IDLE:
						
						SetState(PlayerStates.JUMPING_JOYSTICK);
						
						break;
						
					case PlayerStates.MOVING:
						
						SetState(PlayerStates.JUMPING_JOYSTICK);
						
						break;
						
					case PlayerStates.JUMPING:
						
						break;
						
					case PlayerStates.FALLING:						

						
						break;
						
					case PlayerStates.LANDING:
						
						break;
						
					case PlayerStates.HURT:
						
						break;

					case PlayerStates.ATTACKING_AIR_CHARGING:

						//mCurrentAttackDirection.y = new Vector3(evt.JoystickInfo.AmountX, evt.JoystickInfo.AmountY, 0).normalized.y;

						break;

					case PlayerStates.ATTACKING_GROUND_CHARGING:

						//mCurrentAttackDirection.y = new Vector3(evt.JoystickInfo.AmountX, evt.JoystickInfo.AmountY, 0).normalized.y;

						break;
					}
				}
				else if (evt.JoystickInfo.AmountY <= 0 && mPlayerState == PlayerStates.JUMPING_JOYSTICK)
				{
					SetState(PlayerStates.FALLING);
				}
				
				if (evt.JoystickInfo.AmountY < 0)
				{
					if (mPlayerState == PlayerStates.FALLING && !mIsDropping)
					{
						mIsDropping = true;
					}
				}

			}

			if (walking)
			{
				mAnimator.SetBool("Walking", true);
			}
			else if (mAnimator.GetBool("Walking"))
			{
				mAnimator.SetBool("Walking", false);
			}
		}

		if (evt.KeyBind == BrawlerUserInput.Instance.Attack && (evt.Type == UserInputKeyEvent.TYPE.KEYUP || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_UP))
		{
			if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING)
			{
				SetState(PlayerStates.ATTACKING_AIR);
			}
			else if (mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
			{
				SetState(PlayerStates.ATTACKING_GROUND);
			}
		}
	}

	private void Attack(float attackForce, float attackMultiplier, Vector3 attackDirection)
	{
		//float attackDamage = attackForce * attackMultiplier;
		//Vector3 attackVector = (attackDirection == Vector3.zero ? mTransform.right : attackDirection) * attackDamage;

		//EventManager.Instance.Post(new HitEvent(this, PunchBox.collider.bounds, PunchBox.collider.bounds.center, attackDamage, attackVector));

//		foreach(Collider obj in PunchBox.ObjectList)
//		{
//			if (obj.gameObject.GetInstanceID() == gameObject.GetInstanceID())
//			{
//				continue;
//			}
//			
//			if (obj.GetComponent<Rigidbody>() != null)
//			{
//				obj.GetComponent<Rigidbody>().AddForceAtPosition(attackVector, obj.transform.position);
//				
//				Transform go = (Transform)Instantiate(HitParticle, obj.transform.position + new Vector3(0f,0f,-2f), Quaternion.identity);
//				
//				ParticleSystem hitParticle = go.GetComponent<ParticleSystem>();
//				
//				if (hitParticle != null)
//				{
//					hitParticle.startColor = PlayerColor;
//					Destroy (go.gameObject, hitParticle.duration);
//				}
//				
//				go.transform.rotation = //mSpriteRenderer.transform.rotation;
//				
//				if (obj.GetComponentInChildren<BrawlerPlayerComponent>() != null)
//				{
//					obj.GetComponentInChildren<BrawlerPlayerComponent>().Hurt();
//				}
//				
//			}
//		}
	}

	public void OnDrawGizmos()
	{
		if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
		{
			if (mCurrentAttackDirection != Vector3.zero)
			{
				Gizmos.DrawLine(PunchBox.transform.position, PunchBox.transform.position + (mCurrentAttackDirection * (1 + mTimeInState)));
			}
			else
			{
				Gizmos.DrawLine(PunchBox.transform.position, PunchBox.transform.position + (mTransform.right * (1 + mTimeInState)));
			}
		}
	}

	public void SetID(int id)
	{
		mPlayerID = id;
	}

	public void SetGamepadID(int gamepadID)
	{
		mAssociatedGamepad = gamepadID;
	}

	public void SetPlayerColor(Color color)
	{
		mPlayerColor = color;

		SpriteRenderer sprite = gameObject.GetComponentInChildren<SpriteRenderer>();

		if (sprite != null)
		{ 
			GetComponentInChildren<SpriteRenderer>().color = mPlayerColor;
		}
	}

	public void Hurt()
	{
		SetState(PlayerStates.HURT);
	}

	protected override void OnHit(HitEvent hitEvent)
	{
		if (hitEvent.Sender == this) 
		{
			return;
		}

		Debug.Log (hitEvent.HitVector);

		mRigidbody.AddForce (hitEvent.HitVector, ForceMode.VelocityChange);
						
		Transform go = (Transform)Instantiate(HitParticle, hitEvent.HitPoint, Quaternion.identity);
		
		ParticleSystem hitParticle = go.GetComponent<ParticleSystem>();
		
		if (hitParticle != null)
		{
			hitParticle.startColor = PlayerColor;
			Destroy (go.gameObject, hitParticle.duration);
		}
		
		//go.transform.rotation = //mSpriteRenderer.transform.rotation;

		Hurt ();	
						
	}
	
}


