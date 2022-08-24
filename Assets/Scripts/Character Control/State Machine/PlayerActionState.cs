using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * State to enter when a player performs an action
 * Has a certain duration and may enable certain hitboxes
 */
public class PlayerActionState : PlayerBaseState
{
    public PlayerActionState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory){
        
    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }
}
