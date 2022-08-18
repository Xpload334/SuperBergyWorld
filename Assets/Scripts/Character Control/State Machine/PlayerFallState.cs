using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
    
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        
    }

    public override void EnterState()
    {
        Ctx.IsJumping = true;
        
        InitializeSubState();
        Ctx.Animator.SetBool(Ctx.IsFallingHash, true);
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.IsJumping = false;
        
        Ctx.Animator.SetBool(Ctx.IsFallingHash, false);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
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
        float previousVelocity = Ctx.CurrentMovementY;
        Ctx.CurrentMovementY += Ctx.Gravity * Time.deltaTime;
        Ctx.AppliedMovementY = Mathf.Max((previousVelocity + Ctx.CurrentMovementY) * .5f, Ctx.MaxFallVelocity);
    }
}
