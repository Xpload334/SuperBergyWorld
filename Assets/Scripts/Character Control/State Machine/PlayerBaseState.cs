using System;
using UnityEngine;

/*
 * Abstract class for a base player state
 */
public abstract class PlayerBaseState
{
    private bool _isRootState = false; //If the state is the root state or not
    private PlayerStateMachine _ctx; //State machine this state belongs to
    private PlayerStateFactory _factory; //State factory for all state classes

    private PlayerBaseState _currentSubState; //This state's current sub-state
    private PlayerBaseState _currentSuperState; //This state's current super-state
    
    //Setters and getters
    protected bool IsRootState { set { _isRootState = value; }
    }
    public PlayerBaseState CurrentSubState
    {
        get { return _currentSubState; }
    }
    protected PlayerStateMachine Ctx { get { return _ctx; }
    }
    protected PlayerStateFactory Factory { get { return _factory; }
    }
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    
    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        //Exit current state
        ExitState();
        
        //Enter new state
        newState.EnterState();

        if (_isRootState)
        {
            //Switch current state of context
            _ctx.CurrentState = newState;
        } else if (_currentSuperState != null)
        {
            //Set the current super state's sub state to this new state
            _currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

    public bool HasSubState()
    {
        return _currentSubState != null;
    }

    public String ToString()
    {
        return this.GetType().ToString();
    }
}
