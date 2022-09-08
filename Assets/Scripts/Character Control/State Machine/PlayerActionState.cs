using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * State to enter when a player performs an action
 * Has a certain duration, player cannot move
 */
public class PlayerActionState : PlayerBaseState
{
    private Vector2 _movementInputBackup;
    public PlayerActionState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        /*
        Ctx.DisableControls();
        //Find some better way of freezing character movement while animation plays
        _movementInputBackup = Ctx.CurrentMovementInput;
        Ctx.CurrentMovementInput = Vector2.zero;
        */
        
        //Ctx.DisableControls();
        //Ctx.FreezeCurrentMovement();
        InitializeSubState();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.EnableControls();
        //Ctx.UpdateMovement();
    }

    public override void CheckSwitchStates()
    {
        //If action finished running, switch state to grounded
        if (!Ctx.characterAction.actionIsRunning)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState()
    {
        
    }
}
