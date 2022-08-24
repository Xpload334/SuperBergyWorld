using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    {
        IsRootState = true;
        
    }

    public override void EnterState()
    {
        InitializeSubState();
        HandleGravity();
        
    }

    public override void UpdateState()
    {
        //If falling without jumping
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        //If interact key pressed and has interactable object, switch to interact state
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress && Ctx.Interactable != null)
        {
            Ctx.Interactable.Interact(Ctx);
        }
        //If jump pressed and does not require new jump press, switch to jump state
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress && Ctx.Interactable == null)
        {
            SwitchState(Factory.Jump());
        }
        //If player not grounded and not jump pressed, switch to fall state
        else if (!Ctx.CharacterController.isGrounded && !Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Fall());
        }
        
        
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        else
        {
            SetSubState(Factory.Walk());
        }

    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;
    }
}
