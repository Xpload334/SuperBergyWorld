using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory){
        
    }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsMovingHash, true);
    }

    public override void UpdateState()
    {
        HandleMovement();
        HandleAnimations();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
    }

    public override void InitializeSubState()
    {
        
    }

    void HandleMovement()
    {
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * Ctx.speed;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y * Ctx.speed;

        //Ctx.AppliedMovement = Ctx.CurrentMovement;
    }

    void HandleAnimations()
    {
        if (Ctx.shouldUpdateMoveAnims)
        {
            Ctx.animator.SetFloat(Ctx.MoveXHash, Ctx.roundedMovement.x);
            Ctx.animator.SetFloat(Ctx.MoveYHash, Ctx.roundedMovement.y);
        }
        
        if (Ctx.roundedMovement != Vector2.zero)
        {
            Ctx.animator.SetBool(Ctx.IsMovingHash, true);
        }
        else
        {
            Ctx.animator.SetBool(Ctx.IsMovingHash, false);
        }
    }
}
