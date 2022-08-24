using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractInteractable: MonoBehaviour
{
    public InteractableActivationMethod activationMethod;
    public PartyCharacterManager partyManager;
    public int timesInteracted;
    public Collider triggerCollider;
    public bool triggerActive;

    public bool canInteract;

    void Awake()
    {
        partyManager = FindObjectOfType<PartyCharacterManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        //Check if other is the current character
        if(canInteract && !triggerActive && other.TryGetComponent(out PlayerStateMachine player) && player == partyManager.currentCharacter)
        {
            triggerActive = true;
            player.Interactable = this;
            EnterTrigger(player);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        //Check if other is the current character
        if(canInteract && triggerActive && other.TryGetComponent(out PlayerStateMachine player) && player == partyManager.currentCharacter)
        {
            triggerActive = false;
            player.Interactable = null;
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
