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
    private int currentCharacterIndex;

    public List<GameObject> characterObjectList;
    public List<PlayerStateMachine> characters; //Also add as children
    //Change: maybe read in the child objects of this

    public int partySize;
    [Header("Camera")] 
    public CameraTarget cameraTarget;
    
    //Instance of PlayerInput
    [Header("Player Input")] 
    public bool ShouldSwap;
    public PlayerInput _playerInput;
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
        
        _playerInput.CharacterControls.SwapPrevious.started += OnSwapPrevious;
        _playerInput.CharacterControls.SwapPrevious.canceled += OnSwapPrevious;
        
        _playerInput.CharacterControls.SwapNext.started += OnSwapNext;
        _playerInput.CharacterControls.SwapNext.canceled += OnSwapNext;


        cameraTarget = FindObjectOfType<CameraTarget>();
    }

    // Start is called before the first frame update
    void Start()
    {
        initialiseParty();
        initialiseCharacterIndex();
        setupCharacterControl();
    }

    void initialiseParty()
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

                if (player == currentCharacterObject)
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

    void initialiseCharacterIndex()
    {
        //If current not set up, set to 1st element of characters list
        if (currentCharacter == null)
        {
            currentCharacter = characters[0];
            currentCharacterIndex = 0;
        }
        else
        {
            currentCharacterIndex = characters.IndexOf(currentCharacter);
        }
    }

    void setupCharacterControl()
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

    // Update is called once per frame
    void Update()
    {
        
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
    void swapPrevious()
    {
        DeactivateCharacter(characters[currentCharacterIndex]);
        
        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex = characters.Count - 1;
        }
        
        ActivateCharacter(characters[currentCharacterIndex]);
        Debug.Log("SwapPrevious, Party Index = "+currentCharacterIndex);
    }
    void swapNext()
    {
        DeactivateCharacter(characters[currentCharacterIndex]);
        
        currentCharacterIndex++;
        if (currentCharacterIndex >= characters.Count)
        {
            currentCharacterIndex = 0;
        }
        
        ActivateCharacter(characters[currentCharacterIndex]);
        Debug.Log("SwapNext, Party Index = "+currentCharacterIndex);
    }
    
    void OnSwapPrevious(InputAction.CallbackContext ctx)
    {
        _isSwapPreviousPressed = ctx.ReadValueAsButton();
        //Don't press both swap keys at the same time
        if (_isSwapPreviousPressed && !_isSwapNextPressed)
        {
            swapPrevious();
        }
    }
    void OnSwapNext(InputAction.CallbackContext ctx)
    {
        _isSwapNextPressed = ctx.ReadValueAsButton();
        //Don't press both swap keys at the same time
        if (_isSwapNextPressed && !_isSwapPreviousPressed)
        {
            swapNext();
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
