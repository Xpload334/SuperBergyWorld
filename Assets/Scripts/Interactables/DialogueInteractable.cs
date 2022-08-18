using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DialogueActivationMethod
{
    Interact,
    OnStart,
    TriggerZone
}
public class DialogueInteractable : AbstractInteractable
{
    public Queue<int> DialogueIDQueue;
    
    [Header("Dialogue Controls")]
    public DialogueActivationMethod activationMethod;
    public DialogueManager dialogueManager;
    public bool CanReplayDialogue = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void EnterTrigger(PlayerStateMachine player)
    {
        //throw new System.NotImplementedException();
    }

    public override void ExitTrigger(PlayerStateMachine player)
    {
        //throw new System.NotImplementedException();
    }

    public override void Interact(PlayerStateMachine player)
    {
        //Find xml loader
        //Get dialogue object of first string in queue
        //From object, get queue of dialogue lines (which contain the text string, typer and autoskip settings)
        //Send lines to dialogue manager's queue
        //Disable player
        //Play dialogue
        //(Dialogue manager will enable player)
        
        /*
        FindObjectOfType<DialogueLoader>().GetDialogueObject()
        FindObjectOfType<DialogueManager>()
        */
    }

    public void TriggerDialogue()
    {
        dialogueManager.PlayDialogue(DialogueIDQueue.Dequeue());
    }
}
