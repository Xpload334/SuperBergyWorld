using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//General character states
public enum PlayerState
{
    //Able to be controlled
    Active,
    //Currently interacting
    Interacting,
    //Not able to be controlled
    Inactive
}
/*
 * States for a character moving
 * Will still be performed and can change even when a character is not being controlled
 * e.g. when following, will still use the different movement states
 */
public enum PlayerMovementState
{
    //Not moving
    Idle,
    //Moving
    Walk,
    //Jumping (takes priority over IDLE and WALK)
    Jumping,
    //Currently performing an action
    Action
}

/*
 * States for a character not being controlled
 * i.e. what to do when this character is not being controlled anymore
 */
public enum PlayerInactiveState
{
    //Do not move
    Stationary,
    //Follow the current character (single file line?)
    Follow,
    //(Optional: randomly walk around within a small radius)
    Wander,
    //(Optional: follow a set timeline animation, e.g. walking to a specific spot)
    Path
}


/*
 * Class used to hold and communicate the current player state.
 * Holds the general active/inactive/interact state, as well as substates for active and inactive players.
 */
public class PlayerStateController : MonoBehaviour
{
    public PlayerState currentPlayerState;
    public PlayerMovementState currentPlayerMovementState;
    public PlayerInactiveState currentPlayerInactiveState;



    //PlayerState setters
    public void setActiveState()
    {
        currentPlayerState = PlayerState.Active;
    }
    public void setInteractState()
    {
        currentPlayerState = PlayerState.Interacting;
    }
    public void setInactiveState()
    {
        currentPlayerState = PlayerState.Inactive;
    }
    
    //PlayerMovementState setters
    public void setIdleMovementState()
    {
        currentPlayerMovementState = PlayerMovementState.Idle;
    }
    public void setWalkMovementState()
    {
        currentPlayerMovementState = PlayerMovementState.Walk;
    }
    public void setJumpingMovementState()
    {
        currentPlayerMovementState = PlayerMovementState.Jumping;
    }
    public void setActionMovementState()
    {
        currentPlayerMovementState = PlayerMovementState.Action;
    }
    
    //PlayerInactiveState setters
}
