using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")] 
    public bool isOpen;
    public Animator dialogueBoxAnimator;
    public TMP_Text uiText;
    public Image spriteBox;
    [Header("Sound")] 
    public AudioSource dialogueSoundSource;
    [Header("Dialogue")]
    public static DialogueManager Instance;
    public Queue<Dialogue> DialogueQueue = new Queue<Dialogue>();
    private Typewriter _typewriter;
    public float typeDelay = 0.1f;
    public float waitBeforeDialogueStart = 0.5f;
    public bool disableControlOnFinished;
    [Header("Player")]
    public PartyCharacterManager characterManager;
    private PlayerInput _playerInput;
    public bool shouldConfirm;
    public bool isConfirmPressed;
    public bool shouldSkip;
    public bool isSkipPressed;

    public DialogueLoader dialogueLoader;
    private static readonly int Open = Animator.StringToHash("IsOpen");

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        _typewriter = GetComponent<Typewriter>();
        characterManager = FindObjectOfType<PartyCharacterManager>();
        dialogueLoader = FindObjectOfType<DialogueLoader>();
        
        _playerInput = new PlayerInput();
        //Callback context for dialogue controls
        _playerInput.DialogueControls.Confirm.started += OnConfirm;
        _playerInput.DialogueControls.Confirm.canceled += OnConfirm;
        
        _playerInput.DialogueControls.Skip.started += OnSkip;
        _playerInput.DialogueControls.Skip.canceled += OnSkip;

        ClearText();
    }

    /*
     * On confirm pressed, move to next dialogue if typing is finished.
     */
    void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (shouldConfirm)
        {
            isConfirmPressed = ctx.ReadValueAsButton();
        }
    }
    
    /*
     * On skip pressed, stop the typing but don't move to the next dialogue.
     * Only allow skip if the typing is in progress.
     */
    void OnSkip(InputAction.CallbackContext ctx)
    {
        if (shouldSkip)
        {
            isSkipPressed = ctx.ReadValueAsButton();
        }
    }

    public void PlayDialogue(int dialogueID)
    {
        if (!isOpen)
        {
            dialogueBoxAnimator.SetBool(Open, true);
        }
        isOpen = true;
        Debug.Log("Starting dialogue with id="+dialogueID);
        
        ClearDialogueQueue();
        DialogueObject dialogueObject = dialogueLoader.GetDialogueObject(dialogueID);
        DialogueQueue = ConstructDialogue(dialogueObject); // Construct dialogue queue

        //If should resume control on end, make sure to set flag for later
        if (dialogueObject.disableControlOnFinish)
        {
            disableControlOnFinished = true;
        }

        //If should disable control at start (default), disable player controls
        if (dialogueObject.disableControlOnStart)
        {
            DisablePlayerControls();
        }
        
        //Enable dialogue controls (separate from player controls)
        EnableControls();
        
        StartCoroutine(StepThroughDialogue());
    }

    public IEnumerator StepThroughDialogue()
    {
        bool isFirst = true;
        foreach (Dialogue dialogue in DialogueQueue)
        {
            //If playing first dialogue in queue, wait a bit before starting
            if (isFirst)
            {
                _typewriter.waitBeforeDialogueStart = waitBeforeDialogueStart;
                isFirst = false;
            }
            else
            {
                _typewriter.waitBeforeDialogueStart = 0;
            }
            yield return RunTypingEffect(dialogue);
            
            uiText.text = dialogue.Text; //Set to full text once skipped
            yield return null;
            
            //After dialogue is finished
            if (dialogue.ShouldAutoSkip)
            {
                yield return new WaitForSeconds(dialogue.AutoSkipWait);
            }
            else
            {
                yield return new WaitUntil((() => isConfirmPressed));
            }
            //After auto skip or confirmed, move to next dialogue
        }

        yield return EndDialogue();
        //StartCoroutine(EndDialogue());
    }

    public IEnumerator RunTypingEffect(Dialogue dialogue)
    {
        _typewriter.Run(dialogue.Text, uiText, dialogue.Typer);
        while(_typewriter.IsRunning)
        {
            yield return null;
            
            //Press X to skip dialogue
            //Skip can only happen if dialogue doesn't auto-skip
            if (isSkipPressed && !dialogue.ShouldAutoSkip)
            {
                _typewriter.Stop();
            }
        }
        
    }
    
    /*
     * Actions to perform on dialogue object ending
     */
    public IEnumerator EndDialogue()
    {
        if (isOpen)
        {
            dialogueBoxAnimator.SetBool(Open, false);
        }
        isOpen = false;
        Debug.Log("End of dialogue");
        
        //If should resume control when dialogue is finished, do so
        if (!disableControlOnFinished)
        {
            //Re-enable player controls
            characterManager.currentCharacter.EnableControls();
            //Re-enable party controls
            characterManager.EnableControls();
            
            disableControlOnFinished = false;
        }
        
        //Disable dialogue controls
        DisableControls();
        
        yield break;
    }

    public void ClearDialogueQueue()
    { 
        DialogueQueue.Clear();
    }
    
    //Construct the queue of Dialogue classes, which contains text and the typer information
    private Queue<Dialogue> ConstructDialogue(DialogueObject dialogueObject)
    {
         Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
         foreach (var line in dialogueObject.lines)
         {
              Dialogue dialogue = new Dialogue
              {
                  Text = line.text,
                  Typer = ConstructTyperContents(line.typerID),
                  ShouldAutoSkip = line.ShouldAutoSkip,
                  AutoSkipWait = line.autoSkipSeconds
              };

              dialogueQueue.Enqueue(dialogue);
         }
         
         return dialogueQueue;
    }

    //Construct the Typer Contents class from a given typer ID, includes sprites and sound
    private TyperContents ConstructTyperContents(int typerID)
    {
        Typer typer = dialogueLoader.GetTyper(typerID);

        TyperContents typerContents = new TyperContents
        {
            SpriteList = dialogueLoader.GetTyperSprites(typer),
            SpriteInterval = typer.soundInterval,
            //Debug.Log("Finding sound with ID="+typer.soundID);
            Sound = dialogueLoader.GetSound(typer.soundID),
            SoundInterval = typer.soundInterval,
            TyperName = typer.typerName
        };

        return typerContents;
    }

    public void ClearText()
    {
        uiText.text = "";
    }
    
    private void OnEnable()
    {
        EnableControls();
    }

    private void OnDisable()
    {
        DisableControls();
    }

    private void EnableControls()
    {
        _playerInput.DialogueControls.Enable();
    }

    private void DisableControls()
    {
        _playerInput.DialogueControls.Disable();
    }

    private void DisablePlayerControls()
    {
        //Disable player controls
        characterManager.currentCharacter.DisableControls();
        
        //Zero player movement inputs
        characterManager.currentCharacter.CurrentMovementInput = Vector2.zero;
        
        //Disable party controls
        characterManager.DisableControls();
    }
    

    ///////////////////////////////////
    /*
     * On start playing dialogue:
     *
     * Make sure all players grounded
     * Set all players to interact state
     * Using DialogueLoader, find dialogue object with the given name and load the Dialogue into the queue
     */

    /*
     * For each dialogue line:
     *
     * Load image list and set index to 0
     * Load sound file 
     * Load string text
     * Start typewriter effect
     */
    
    /*
     * When dialogue line finished:
     *
     * Disable UI
     * Set all players to idle state
     */
}
