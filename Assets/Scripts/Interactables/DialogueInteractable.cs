using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class DialogueInteractable : AbstractInteractable
{
    public List<int> dialogueIDList;
    private Queue<int> _dialogueIDQueue;
    [Header("Dialogue Controls")]
    
    public DialogueManager dialogueManager;

    [Header("Context Clue")] 
    public GameObject contextClue;
    /*
     * If there is 1 object left in the dialogue queue and this bool is true, this dialogue will be replayed
     * on every subsequent interaction.
     */
    public bool canReplayDialogue = true;
    
    // Start is called before the first frame update
    void Start()
    {
        //initialise queue from list
        _dialogueIDQueue = new Queue<int>(dialogueIDList);
        dialogueManager = FindObjectOfType<DialogueManager>();


        //If starts on trigger, dialogue cannot replay
        if (activationMethod == InteractableActivationMethod.TriggerZone)
        {
            canReplayDialogue = false;
        }
    }

    protected override void EnterTrigger(PlayerStateMachine player)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Trigger Entered");

        if (_dialogueIDQueue.Count > 0)
        {
            //If activates on trigger zone, interact upon entering trigger
            if (activationMethod == InteractableActivationMethod.TriggerZone)
            {
                Interact(player);
            }
            else
            {
                SetContextClueActive(true);
            }
        }
    }

    protected override void ExitTrigger(PlayerStateMachine player)
    {
        //throw new System.NotImplementedException();
        Debug.Log("Trigger Exited");
        
        if(activationMethod == InteractableActivationMethod.Interact)
        {
            SetContextClueActive(false);
        }
    }

    public override void Interact(PlayerStateMachine player)
    {
        //If no more dialogue left, do nothing
        if (_dialogueIDQueue.Count == 0)
        {
            return;
        }

        //player.Interactable = null; //Set interactable to null
        SetContextClueActive(false); //Set context clue to inactive
        
        //If trigger active, trigger dialogue
        if (triggerActive && canInteract)
        {
            canInteract = false;
            StartCoroutine(TriggerDialogue());
        }
        
    }

    public IEnumerator TriggerDialogue()
    {
        int dialogueID = _dialogueIDQueue.Dequeue();
        dialogueManager.PlayDialogue(dialogueID);
        
        //Once dialogue is finished, set canInteract back to true
        yield return new WaitUntil((() => !dialogueManager.isOpen));
        EndInteract(dialogueID);
        
    }

    /*
     * Method to play on ending an interaction
     */
    public void EndInteract(int dialogueID)
    {
        //If no more dialogue and can replay dialogue, put last dialogue ID back in the queue
        if (_dialogueIDQueue.Count == 0 && canReplayDialogue)
        {
            _dialogueIDQueue.Enqueue(dialogueID);
        }

        canInteract = true; //Re-enable interactions
    }

    public void SetContextClueActive(bool state)
    {
        contextClue.SetActive(state);
    }
    

    /*
     * Add a new dialogue ID int to the dialogue queue.
     * If there is a replayable element in the queue, preserve its position at the end of the queue
     */
    public void AddDialogueToQueue(int dialogueID)
    {
        //If replayable element left, move to back of the queue and put new dialogue ID in front of it
        if (_dialogueIDQueue.Count == 1 && canReplayDialogue)
        {
            int replayableID = _dialogueIDQueue.Dequeue();
            _dialogueIDQueue.Enqueue(dialogueID); //Put new ID into the queue
            _dialogueIDQueue.Enqueue(replayableID); //Put replayable ID at the end of the queue
        }
    }

    public void ReplaceReplayableID(int dialogueID)
    {
        
    }

    
}
