using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


//Manages the list of characters in the party
//Characters should really be children of the object this script is attached to
//Uses input system to allow to swap characters
public class PartyCharacterManager : MonoBehaviour
{
    public GameObject currentCharacterObject;
    public PlayerStateMachine currentCharacter;
    //private int _currentCharacterIndex;

    [Header("Characters")]
    public List<GameObject> characterObjectList;
    public List<PlayerStateMachine> characters; //Also add as children
    public DoublyLinkedList<int> CharacterIndexDoublyLinkedList = new DoublyLinkedList<int>(); //Keep track of the active characters using this
    //Head of the list is the current character

    //Change: maybe read in the child objects of this
    public int partySize;
    [Header("Camera")] 
    public CameraTarget cameraTarget;
    
    //Instance of PlayerInput
    [Header("Player Input")] 
    public Vector2 currentMovementInput;
    public bool shouldControl;
    public bool shouldSwap;
    private PlayerInput _playerInput;
    
    private bool _isSwapPreviousPressed;
    private bool _requireNewSwapPreviousPressed;
    private bool _isSwapNextPressed;
    private bool _requireNewSwapNextPressed;
    
    //Singleton
    private static PartyCharacterManager _instance;
    public static PartyCharacterManager Instance
    {
        get
        {
            if (_instance is null)
            {
                Debug.LogError("Party Character Manager is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        
        _playerInput = new PlayerInput();
        
        //Swap Previous
        _playerInput.CharacterControls.SwapPrevious.started += OnSwapPrevious;
        _playerInput.CharacterControls.SwapPrevious.canceled += OnSwapPrevious;
        //Swap Next
        _playerInput.CharacterControls.SwapNext.started += OnSwapNext;
        _playerInput.CharacterControls.SwapNext.canceled += OnSwapNext;
        
        //Callback context for movement
        _playerInput.CharacterControls.Movement.started += OnMovementInput;
        _playerInput.CharacterControls.Movement.performed += OnMovementInput;
        _playerInput.CharacterControls.Movement.canceled += OnMovementInput;
        
        //Callback context for jump/interact
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        
        //Callback context for action
        _playerInput.CharacterControls.Action.started += OnAction;
        _playerInput.CharacterControls.Action.canceled += OnAction;


        cameraTarget = FindObjectOfType<CameraTarget>();
    }
    
    /*
     * Take inputs in the party character manager, and pass on to the current character
     */
    //Movement input
    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        if (shouldControl)
        {
            currentMovementInput = ctx.ReadValue<Vector2>();
            currentCharacter.OnMovement(ctx.ReadValue<Vector2>());
        }
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (shouldControl)
        {
            currentCharacter.IsJumpPressed = ctx.ReadValueAsButton();
            currentCharacter.RequireNewJumpPress = false;
        }
    }
    
    void OnAction(InputAction.CallbackContext ctx)
    {
        if (shouldControl && currentCharacter.characterAction != null)
        {
            currentCharacter.IsActionPressed = ctx.ReadValueAsButton();
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        InitialiseParty();
        InitialiseCharacterIndex();
        SetupCharacterControl();
    }

    void InitialiseParty()
    {
        /*
        //If object list doesn't contain current character
        if (!characterObjectList.Contains(currentCharacterObject))
        {
            characterObjectList.Add(currentCharacterObject);
            Debug.Log("Object list doesn't contain current character, added to list.");
        }
        */

        int i = 0;
        //Add each player in object list to player list
        foreach (GameObject characterObject in characterObjectList)
        {
            //If doesn't have player script, log error
            if (characterObject.TryGetComponent(out PlayerStateMachine player).Equals(true))
            {
                characters.Add(player);
                CharacterIndexDoublyLinkedList.AddLast(i); //Add index to linked list
                Debug.Log(player.name+" added to party.");
                i++;
            }
            else
            {
                Debug.LogError(player.name+" is not a valid player.");
            }
        }
    }

    void InitialiseCharacterIndex()
    {
        //If current not set up, set to 1st element of characters list
        currentCharacter = characters[0];
    }

    void SetupCharacterControl()
    {
        PlayerStateMachine lastCharacter = characters.Last();
        foreach (var character in characters)
        {
            if (character == currentCharacter)
            {
                ActivateCharacter(character);
                character.DisablePathfinding();
            }
            else
            {
                DeactivateCharacter(character);
                character.FollowTarget(lastCharacter.transform); //Follow the last character
            }

            lastCharacter = character;
        }
    }

    public void ActivateCharacter(PlayerStateMachine character)
    {
        character.Activate();
        Debug.Log(character +" is now active");
        cameraTarget.LockTo(character.gameObject);
    }

    public void DeactivateCharacter(PlayerStateMachine character)
    {
        character.Deactivate();
        character.OnMovement(Vector2.zero);
        Debug.Log(character +" is now inactive");
        
        //Make character follow currently active character
    }
    
    //Cycle between characters in the party
    //Q and E to do Previous and Next
    void SwapPrevious()
    {
        /*
         * 1 <- 2 <- 3
         * goes to
         * 3 <- 1 <- 2
         */
        
        int headIndex = CharacterIndexDoublyLinkedList.First.Value;
        int previousIndex = CharacterIndexDoublyLinkedList.Last.Value; //Previous, in this case, being the last element of the list
        
        //Deactivate control of head character
        DeactivateCharacter(characters[headIndex]);
        //Activate control of last character
        ActivateCharacter(characters[previousIndex]);
        currentCharacter = characters[previousIndex];
        currentCharacterObject = currentCharacter.gameObject;
        //Disable pathfinding of last character
        characters[previousIndex].DisablePathfinding();
        
        //Add last index to the front
        CharacterIndexDoublyLinkedList.AddFirst(previousIndex);
        //Remove last index
        CharacterIndexDoublyLinkedList.RemoveLast();
        
        
        //Previous head of list will now pathfind towards the new head
        headIndex = CharacterIndexDoublyLinkedList.First.Value; //Head of the list
        int oldHeadIndex = CharacterIndexDoublyLinkedList.First.Next.Value; //Original head of the list
        characters[oldHeadIndex].FollowTarget(characters[headIndex].transform); //Follow next character in the line
        
        Debug.Log("SwapPrevious, Party Index = "+previousIndex);
    }
    void SwapNext()
    {
        /*
         * 1 <- 2 <- 3
         * goes to
         * 2 <- 3 <- 1
         */

        int headIndex = CharacterIndexDoublyLinkedList.First.Value;
        int nextIndex = CharacterIndexDoublyLinkedList.First.Next.Value;
        
        //Deactivate control of head character
        DeactivateCharacter(characters[headIndex]);
        //Activate control of next character
        ActivateCharacter(characters[nextIndex]);
        currentCharacter = characters[nextIndex];
        currentCharacterObject = currentCharacter.gameObject;
        //Disable pathfinding of next character
        characters[nextIndex].DisablePathfinding();
        
        //Add headIndex to end
        CharacterIndexDoublyLinkedList.AddLast(headIndex);
        //Remove headIndex
        CharacterIndexDoublyLinkedList.RemoveFirst();
        
        //Last index will now pathfind towards their previous index in the linked list
        int newTargetIndex = CharacterIndexDoublyLinkedList.Last.Previous.Value; //Next character in the line
        int lastIndex = CharacterIndexDoublyLinkedList.Last.Value; //Index of last character (previous head)
        characters[lastIndex].FollowTarget(characters[newTargetIndex].transform); //Follow next character in the line
        
        Debug.Log("SwapNext, Party Index = "+nextIndex);
    }
    
    void OnSwapPrevious(InputAction.CallbackContext ctx)
    {
        _isSwapPreviousPressed = ctx.ReadValueAsButton();
        //Don't press both swap keys at the same time
        if (_isSwapPreviousPressed && !_isSwapNextPressed)
        {
            SwapPrevious();
        }
    }
    void OnSwapNext(InputAction.CallbackContext ctx)
    {
        _isSwapNextPressed = ctx.ReadValueAsButton();
        //Don't press both swap keys at the same time
        if (_isSwapNextPressed && !_isSwapPreviousPressed)
        {
            SwapNext();
        }
    }
    
    private void OnEnable()
    {
        EnableControls();
    }

    private void OnDisable()
    {
        DisableControls();
    }

    public void EnableControls()
    {
        _playerInput.CharacterControls.Enable();
    }

    public void DisableControls()
    {
        _playerInput.CharacterControls.Disable();
    }


}
