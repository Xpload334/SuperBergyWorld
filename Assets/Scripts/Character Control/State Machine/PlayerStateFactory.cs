using System;
using System.Collections.Generic;
using UnityEngine;

enum PlayerStates
{
    Idle,
    Walk,
    Jump,
    Grounded,
    Fall,
    Action,
    Follow,
    Interact
}
public class PlayerStateFactory
{
    private PlayerStateMachine _context;
    private Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        ConstructStates();
    }

    void ConstructStates()
    {
        _states.Add(PlayerStates.Idle, new PlayerIdleState(_context, this));
        _states.Add(PlayerStates.Walk, new PlayerWalkState(_context, this));
        _states.Add(PlayerStates.Jump, new PlayerJumpState(_context, this));
        _states.Add(PlayerStates.Grounded, new PlayerGroundedState(_context, this));
        _states.Add(PlayerStates.Fall, new PlayerFallState(_context, this));
        _states.Add(PlayerStates.Action, new PlayerActionState(_context, this));
        _states.Add(PlayerStates.Follow, new PlayerFollowState(_context, this));
        _states.Add(PlayerStates.Interact, new PlayerInteractState(_context, this));

        /*
        _states[PlayerStates.Idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.Walk] = new PlayerWalkState(_context, this);
        _states[PlayerStates.Jump] = new PlayerJumpState(_context, this);
        _states[PlayerStates.Grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.Fall] = new PlayerFallState(_context, this);
        _states[PlayerStates.Action] = new PlayerActionState(_context, this);
        _states[PlayerStates.Follow] = new PlayerFollowState(_context, this);
        _states[PlayerStates.Interact] = new PlayerInteractState(_context, this);
        */
        
        Debug.Log("States constructed");
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.Idle];
    }
    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.Walk];
    }

    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.Jump];
    }

    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.Grounded];
    }

    public PlayerBaseState Fall()
    {
        return _states[PlayerStates.Fall];
    }

    //custom
    public PlayerBaseState Action()
    {
        return _states[PlayerStates.Action];
    }

    public PlayerBaseState Follow()
    {
        return _states[PlayerStates.Follow];
    }

    public PlayerBaseState Interact()
    {
        return _states[PlayerStates.Interact];
    }
}
