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
    private int _currentCharacterIndex;

    public List<GameObject> characterObjectList;
    public List<PlayerStateMachine> characters; //Also add as children
    //Change: maybe read in the child objects of this

    public int partySize;
    [Header("Camera")] 
    public CameraTarget cameraTarget;
    
    //Instance of PlayerInput
    [Header("Player Input")] 
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
        if (shouldControl)
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
        //If object list doesn't contain current character
        if (!characterObjectList.Contains(currentCharacterObject))
        {
            characterObjectList.Add(currentCharacterObject);
            Debug.Log("Object list doesn't contain current character, added to list.");
        }

        //Add each player in object list to player list
        foreach (GameObject characterObject in characterObjectList)
        {
            //If doesn't have player script, log error
            if (characterObject.TryGetComponent(out PlayerStateMachine player).Equals(true))
            {
                characters.Add(player);
                Debug.Log(player.name+" added to party.");

                if (currentCharacterObject == characterObject)
                {
                    currentCharacter = player;
                }
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
        if (currentCharacter == null)
        {
            currentCharacter = characters[0];
            _currentCharacterIndex = 0;
        }
        else
        {
            _currentCharacterIndex = characters.IndexOf(currentCharacter);
        }
    }

    void SetupCharacterControl()
    {
        foreach (var character in characters)
        {
            if (character == currentCharacter)
            {
                ActivateCharacter(character);
            }
            else
            {
                DeactivateCharacter(character);
            }
            
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
        Debug.Log(character +" is now inactive");
    }
    
    //Cycle between characters in the party
    //Q and E to do Previous and Next
    void SwapPrevious()
    {
        DeactivateCharacter(characters[_currentCharacterIndex]);
        
        _currentCharacterIndex--;
        if (_currentCharacterIndex < 0)
        {
            _currentCharacterIndex = characters.Count - 1;
        }
        
        ActivateCharacter(characters[_currentCharacterIndex]);
        Debug.Log("SwapPrevious, Party Index = "+_currentCharacterIndex);
    }
    void SwapNext()
    {
        DeactivateCharacter(characters[_currentCharacterIndex]);
        
        _currentCharacterIndex++;
        if (_currentCharacterIndex >= characters.Count)
        {
            _currentCharacterIndex = 0;
        }
        
        ActivateCharacter(characters[_currentCharacterIndex]);
        Debug.Log("SwapNext, Party Index = "+_currentCharacterIndex);
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
