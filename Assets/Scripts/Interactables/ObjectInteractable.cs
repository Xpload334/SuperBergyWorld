using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : AbstractInteractable
{
    public ITriggerFromInteract objectToTrigger;

    protected override void EnterTrigger(PlayerStateMachine player)
    {
        //throw new System.NotImplementedException();
        //If activates on trigger zone, interact upon entering trigger
        if (activationMethod == InteractableActivationMethod.TriggerZone)
        {
            Interact(player);
        }
    }

    protected override void ExitTrigger(PlayerStateMachine player)
    {
        //throw new System.NotImplementedException();
    }

    public override void Interact(PlayerStateMachine player)
    {
        if (triggerActive && canInteract)
        {
            objectToTrigger.TriggerAction();
        }
    }
}
