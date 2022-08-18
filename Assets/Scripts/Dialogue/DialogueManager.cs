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
    public bool IsOpen;
    public Animator dialogueBoxAnimator;
    public TMP_Text uiText;
    public Image spriteBox;
    [Header("Sound")] 
    public AudioSource dialogueSoundSource;
    [Header("Dialogue")]
    public static DialogueManager instance;
    public Queue<Dialogue> DialogueQueue = new Queue<Dialogue>();
    private Typewriter _typewriter;
    public float typeDelay = 0.1f;
    public float waitBeforeDialogueStart = 0.5f;
    [Header("Player")]
    public PartyCharacterManager characterManager;
    private PlayerInput _playerInput;
    public bool shouldConfirm;
    public bool _isConfirmPressed;
    public bool shouldSkip;
    public bool _isSkipPressed;

    public DialogueLoader dialogueLoader;
    private static readonly int Open = Animator.StringToHash("IsOpen");

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
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

    }

    private void Update()
    {
        
    }

    /*
     * On confirm pressed, move to next dialogue if typing is finished.
     */
    void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (shouldConfirm)
        {
            _isConfirmPressed = ctx.ReadValueAsButton();
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
            _isSkipPressed = ctx.ReadValueAsButton();
        }
    }

    public void PlayDialogue(int dialogueID)
    {
        if (!IsOpen)
        {
            dialogueBoxAnimator.SetBool(Open, true);
        }
        IsOpen = true;
        Debug.Log("Starting dialogue with id="+dialogueID);
        
        ClearDialogueQueue();
        DialogueObject dialogueObject = dialogueLoader.GetDialogueObject(dialogueID);
        DialogueQueue = ConstructDialogue(dialogueObject); // Construct dialogue queue

        //Disable player controls
        characterManager.currentCharacter.DisableControls();
        //Disable party controls
        characterManager.DisableControls();
        //Enable dialogue controls
        EnableControls();
        
        StartCoroutine(StepThroughDialogue());
        //DisplayNextLine();
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
                yield return new WaitUntil((() => _isConfirmPressed));
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
            //Press Z again to skip dialogue
            if (_isSkipPressed)
            {
                _typewriter.Stop();
                //DialogueSkipped = false;
            }
        }
        
    }
    
    /*
     * Actions to perform on dialogue object ending
     */
    public IEnumerator EndDialogue()
    {
        if (IsOpen)
        {
            dialogueBoxAnimator.SetBool(Open, false);
        }
        IsOpen = false;
        Debug.Log("End of dialogue");
        
        
        //Re-enable player controls
        characterManager.currentCharacter.EnableControls();
        //Re-enable party controls
        characterManager.EnableControls();
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
              Dialogue dialogue = new Dialogue();
              dialogue.Text = line.text;
              dialogue.Typer = ConstructTyperContents(line.typerID);

              dialogue.ShouldAutoSkip = line.ShouldAutoSkip;
              dialogue.AutoSkipWait = line.autoSkipSeconds;
              
              dialogueQueue.Enqueue(dialogue);
         }
         
         return dialogueQueue;
    }

    //Construct the Typer Contents class from a given typer ID, includes sprites and sound
    private TyperContents ConstructTyperContents(int typerID)
    {
        Typer typer = dialogueLoader.GetTyper(typerID);

        TyperContents typerContents = new TyperContents();
        typerContents.spriteList = dialogueLoader.GetTyperSprites(typer);
        typerContents.SpriteInterval = typer.soundInterval;

        //Debug.Log("Finding sound with ID="+typer.soundID);
        typerContents.Sound = dialogueLoader.GetSound(typer.soundID);
        typerContents.SoundInterval = typer.soundInterval;

        typerContents.TyperName = typer.typerName;
        return typerContents;
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
