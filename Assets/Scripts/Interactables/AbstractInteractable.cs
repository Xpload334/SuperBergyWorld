using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractInteractable: MonoBehaviour
{
    private PartyCharacterManager _partyManager;
    public int timesInteracted;
    public Collider myCollider;
    public bool triggerActive;

    public bool canInteract;

    void Awake()
    {
        _partyManager = FindObjectOfType<PartyCharacterManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        //Check if other is the current character
        if(canInteract && !triggerActive && other.TryGetComponent(out PlayerStateMachine player) && player == _partyManager.currentCharacter)
        {
            triggerActive = true;
            EnterTrigger(player);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        //Check if other is the current character
        if(canInteract && triggerActive && other.TryGetComponent(out PlayerStateMachine player) && player == _partyManager.currentCharacter)
        {
            triggerActive = false;
            ExitTrigger(player);
        }
    }
    
    //Abstract method to handle when an interactable's trigger is entered
    public abstract void EnterTrigger(PlayerStateMachine player);
    //Abstract method to handle when an interactable's trigger is exited
    public abstract void ExitTrigger(PlayerStateMachine player);


    /*
     * Interact with this object
     */
    public abstract void Interact(PlayerStateMachine player);
}
