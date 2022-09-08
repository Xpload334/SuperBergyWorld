using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : MonoBehaviour
{
    public PlayerStateMachine player;
    private Animator _playerAnimator;
    private int _isAttackHash;

    [Header("Running Bools")]
    public bool actionIsRunning; //If the action is currently running (in action state)
    public bool canPerformAction; //If the action key can be pressed again (in any state)

    [Header("Action Modifiers")] 
    private float characterSpeed;
    public float characterSpeedForAction = 0f;
    public bool switchesIntoActionState; //In the action state, cannot move, disable for state changing actions that do not disable movement
    public bool requiresGrounded; //Requires player to be grounded to perform
    public bool canMove;
    
    public float cooldownTime; //Time before the action state can be entered again
    // Start is called before the first frame update
    void Start()
    {
        _playerAnimator = player.animator;

        _isAttackHash = player.IsPerformingActionHash;
    }
    public void StartAction()
    {
        //Debug.Log("Performing action");

        //Speed
        characterSpeed = player.speed;
        player.speed = characterSpeedForAction;
        //Action bools
        actionIsRunning = true;
        canPerformAction = false;
        //Animator bool
        _playerAnimator.SetBool(_isAttackHash, true);
        player.shouldUpdateMoveAnims = false;
    }

    
    //Replace with animation events
    public void EndAction()
    {
        //Debug.Log("End action");
        
        //Character speed
        player.speed = characterSpeed;
        
        actionIsRunning = false;
        //Animator bool
        _playerAnimator.SetBool(_isAttackHash, false);
        player.shouldUpdateMoveAnims = true;
        //player.FreezeCurrentMovement();
        StartCoroutine(Cooldown());
        
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        canPerformAction = true;
    }

    public virtual void TriggerActionEffect()
    {
        //Debug.Log("Action trigger, whack");
    }
    
    
    
}
