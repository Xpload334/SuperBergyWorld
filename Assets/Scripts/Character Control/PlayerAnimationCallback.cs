using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationCallback : MonoBehaviour
{
    public PlayerStateMachine player;
    

    public void ActionTrigger()
    {
        //Trigger the important part of the action
        if (player.characterAction != null)
        {
            player.characterAction.TriggerActionEffect();
        }
    }

    public void ActionEnd()
    {
        //On animation finished
        if (player.characterAction != null)
        {
            player.characterAction.EndAction();
        }
    }
}
