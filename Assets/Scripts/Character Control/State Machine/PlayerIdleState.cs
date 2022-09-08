using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory){
        
    }

    public override void EnterState()
    {
        Ctx.animator.SetFloat(Ctx.MoveXHash, Mathf.Round(Ctx.LastMovementX));
        Ctx.animator.SetFloat(Ctx.MoveYHash, Mathf.Round(Ctx.LastMovementY));
        
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
        
        Ctx.Animator.SetBool(Ctx.IsMovingHash, false);
    }

    public override void UpdateState()
    {
        HandleAnimations();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void InitializeSubState()
    {
        
    }
    void HandleAnimations()
    {
        Ctx.animator.SetBool(Ctx.IsMovingHash, false);
    }
}
