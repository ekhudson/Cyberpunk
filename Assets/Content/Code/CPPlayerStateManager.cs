using UnityEngine;
using System.Collections;

public class CPPlayerStateManager : MonoBehaviour 
{
    public enum PlayerStates
    {
        IDLE,
        IDLE_COLLIDE, //Hit by something else?
        IDLE_HURT,
        IDLE_JUMP, //Jump straight up
        IDLE_DRAW_GUN,
        IDLE_ATTACK,
        IDLE_SHOOT,
        IDLE_RELOAD,
        IDLE_CROUCH,
        IDLE_CROUCH_COLLIDE,
        IDLE_CROUCH_HURT,
        IDLE_CROUCH_ATTACK,
        IDLE_CROUCH_SHOOT,
        WALKING,
        WALK_TURN,
        WALK_BRAKING,
        WALK_COLLIDE,
        WALK_HURT,
        WALK_JUMP, //short hop
        WALK_JUMP_FALL,
        WALK_JUMP_LAND,
        WALK_DRAW_GUN,
        WALK_ATTACK,
        WALK_SHOOT,
        WALK_RELOAD,
        WALK_CROUCH,
        WALK_CROUCH_COLLIDE,
        WALK_CROUCH_HURT,
        WALK_CROUCH_ATTACK,
        WALK_CROUCH_SHOOT,
        ROLL,
        ROLL_COLLIDE,
        ROLL_HURT,
        RUNNING,
        RUN_TURN,
        RUN_BRAKING,
        RUN_COLLIDE,
        RUN_HURT,
        RUN_JUMP,
        RUN_JUMP_FALL,
        RUN_JUMP_LAND,
        RUN_DRAW_GUN,
        RUN_ATTACK,
        RUN_SHOOT,
        RUN_RELOAD,
    }

    private PlayerStates mPlayerState = PlayerStates.IDLE;
    private float mStateStartTime;

    public PlayerStates PlayerState
    {
        get
        {
            return mPlayerState;
        }
    }

    public float TimeInState
    {
        get
        {
            return (Time.realtimeSinceStartup - mStateStartTime);
        }
    }

    public bool SetState(PlayerStates newState, object sender)
    {
        return SetState(newState, sender, false);
    }

    public bool SetState(PlayerStates newState, object sender, bool carryOverTime)
    {
        if (newState == mPlayerState)
        {
            return false;
        }

        switch (newState)
        {
            case PlayerStates.IDLE:
            break;
            case PlayerStates.IDLE_COLLIDE:
            break;
            case PlayerStates.IDLE_HURT:
            break;
            case PlayerStates.IDLE_JUMP:
            break;
            case PlayerStates.IDLE_DRAW_GUN:
            break;
            case PlayerStates.IDLE_ATTACK:
            break;
            case PlayerStates.IDLE_SHOOT:
            break;
            case PlayerStates.IDLE_RELOAD:
            break;
            case PlayerStates.IDLE_CROUCH:
            break;
            case PlayerStates.IDLE_CROUCH_COLLIDE:
            break;
            case PlayerStates.IDLE_CROUCH_HURT:
            break;
            case PlayerStates.IDLE_CROUCH_ATTACK:
            break;
            case PlayerStates.IDLE_CROUCH_SHOOT:
            break;
            case PlayerStates.WALKING:
            break;
            case PlayerStates.WALK_TURN:
            break;
            case PlayerStates.WALK_BRAKING:
            break;
            case PlayerStates.WALK_COLLIDE:
            break;
            case PlayerStates.WALK_HURT:
            break;
            case PlayerStates.WALK_JUMP: 
            break;
            case PlayerStates.WALK_JUMP_FALL:
            break;
            case PlayerStates.WALK_JUMP_LAND:
            break;
            case PlayerStates.WALK_DRAW_GUN:
            break;
            case PlayerStates.WALK_ATTACK:
            break;
            case PlayerStates.WALK_SHOOT:
            break;
            case PlayerStates.WALK_RELOAD:
            break;
            case PlayerStates.WALK_CROUCH:
            break;
            case PlayerStates.WALK_CROUCH_COLLIDE:
            break;
            case PlayerStates.WALK_CROUCH_HURT:
            break;
            case PlayerStates.WALK_CROUCH_ATTACK:
            break;
            case PlayerStates.WALK_CROUCH_SHOOT:
            break;
            case PlayerStates.ROLL:
            break;
            case PlayerStates.ROLL_COLLIDE:
            break;
            case PlayerStates.ROLL_HURT:
            break;
            case PlayerStates.RUNNING:
            break;
            case PlayerStates.RUN_TURN:
            break;
            case PlayerStates.RUN_BRAKING:
            break;
            case PlayerStates.RUN_COLLIDE:
            break;
            case PlayerStates.RUN_HURT:
            break;
            case PlayerStates.RUN_JUMP:
            break;
            case PlayerStates.RUN_JUMP_FALL:
            break;
            case PlayerStates.RUN_JUMP_LAND:
            break;
            case PlayerStates.RUN_DRAW_GUN:
            break;
            case PlayerStates.RUN_ATTACK:
            break;
            case PlayerStates.RUN_SHOOT:
            break;
            case PlayerStates.RUN_RELOAD:
            break;
            default:
            break;
        }

        if (!carryOverTime)
        {
            mStateStartTime = Time.realtimeSinceStartup;
        }

        mPlayerState = newState;
        return true;
    }
}
