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
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress && Ctx.interactable != null)
        {
            Ctx.interactable.Interact(Ctx);
        }
        //If jump pressed and does not require new jump press, switch to jump state
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress && Ctx.interactable == null)
        {
            SwitchState(Factory.Jump());
        }
        
        //If action pressed and action able to be performed again
        if (Ctx.IsActionPressed && Ctx.characterAction.canPerformAction)
        {
            TryPerformAction();
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


    /*
     * Helper method to run checks if the character's action can be performed
     */
    void TryPerformAction()
    {
        //Run checks on if the action can be performed
        bool canPerform = false;
        //If requires grounded, check if grounded
        if (Ctx.characterAction.requiresGrounded && Ctx.CharacterController.isGrounded)
        {
            //If requires grounded and is grounded
            canPerform = true;
        }
        else if(Ctx.characterAction.requiresGrounded && !Ctx.CharacterController.isGrounded)
        {
            //If requires grounded and not grounded
            //Set to false
        }
        else if(!Ctx.characterAction.requiresGrounded)
        {
            canPerform = true;
        }
        
        //If can perform
        if (canPerform)
        {
            Ctx.characterAction.StartAction(); //Start action
            //If switches into action state, switch states and perform action
            if (Ctx.characterAction.switchesIntoActionState)
            {
                SwitchState(Factory.Action()); //Switch into action state
            }
        }
    }
}
