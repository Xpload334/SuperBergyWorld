using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    //PlayerStateController _playerStateController;
    //Movement variables
    [Header("Movement Variables")]
    public bool shouldMove = true;
    public bool _isMovementPressed;
    public float speed = 5.0f;
    
    //Gravity variables
    [Header("Gravity Variables")]
    public bool hasGravity = true;
    public float gravityValue = -10f;
    private float _groundedGravity = -0.05f;
    
    //Jumping variables
    [Header("Jumping")]
    public bool shouldJump = true;
    public bool shouldAdjustJump = true;
    private bool _isJumping; //bool is true when airborne
    bool _isJumpPressed;
    public float initialJumpVelocity;
    public float maxJumpHeight = 1.0f;
    public float maxJumpTime = 0.75f;
    public float maxFallVelocity = -10.0f;
    private float _fallMultiplier = 2.0f;
    
    //Interaction variables
    [Header("Interactions")] 
    public bool shouldInteract;

    //Action variables
    [Header("Action")]
    public bool shouldAction;
    
    //Party variables
    [Header("Party and Swapping")] 
    public bool shouldFollow;
    
    //Animator
    [Header("Animation")]
    public Animator animator;

    public Vector2 roundedMovement;
    public bool shouldAnimate = true;
    //Variables to store animation parameter IDs
    private int _isMovingHash;
    private int _isJumpingHash;
    private bool _requireNewJumpPress;
    private int _moveXHash;
    private int _moveYHash;
    private int _isFallingHash;
    
    [Header("Components")] 
    private CharacterController _characterController;
    
    //Instance of PlayerInput
    [Header("Player Input")] 
    public bool shouldControl; 
    PlayerInput _playerInput;

    //Storing player input values
    [Header("Input Vectors")]
    public Vector2 _currentMovementInput; 
    public Vector3 _currentMovement;
    Vector3 _appliedMovement;
    Vector3 _lastMovement;
    
    //State variables
    [Header("State Variables")] 
    public PlayerBaseState _currentState;

    public String currentStateName;
    public String currentSubStateName;
    private PlayerStateFactory _states;
    
    //Setters and getters
    public PlayerBaseState CurrentState {
        get { return _currentState; }
        set { _currentState = value;  }
    }
    public Animator Animator { get { return animator; }
    }

    public CharacterController CharacterController { get { return _characterController; }
    }
    public int IsMovingHash { get { return _isMovingHash; }
    }
    public int IsJumpingHash { get { return _isJumpingHash; }
    }
    public int MoveXHash { get { return _moveXHash; }
    }
    public int MoveYHash { get { return _moveYHash; }
    }
    public int IsFallingHash { get { return _isFallingHash; }
    }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; }
    }
    public bool IsJumping { get { return _isJumping; } set { _isJumping = value; }
    }
    public bool IsJumpPressed { get { return _isJumpPressed; } set => _isJumpPressed = value;
    }
    public float Gravity { get { return gravityValue; }
    }
    public float MaxFallVelocity { get { return maxFallVelocity; }
    }
    public bool IsMovementPressed { get { return _isMovementPressed; }
    }
    public float CurrentMovementX { get { return _currentMovement.x;} set { _currentMovement.x = value; }
    }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; }
    }
    public float CurrentMovementZ { get { return _currentMovement.z; } set { _currentMovement.z = value; }
    }
    public float AppliedMovementX { get { return _appliedMovement.x;} set { _appliedMovement.x = value; }
    }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; }
    }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; }
    }
    public float LastMovementX { get { return _lastMovement.x;} set { _lastMovement.x = value; }
    }
    public float LastMovementY { get { return _lastMovement.z; } set { _lastMovement.z = value; }
    }
    public Vector3 CurrentMovement { get { return _currentMovement; } set { _currentMovement = value; }
    } 
    public Vector3 AppliedMovement { get { return _appliedMovement; } set { _appliedMovement = value; }
    }
    public Vector3 LastMovement { get { return _lastMovement; } set { _lastMovement = value; }
    }
    public float GroundedGravity { get { return _groundedGravity; }
    }
    public float FallMultiplier { get { return _fallMultiplier; }
    }
    public float Speed { get { return speed; }
    }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } set => _currentMovementInput = value;
    }

    private void Awake()
    {
        //Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        _isMovingHash = Animator.StringToHash("IsMoving");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _moveXHash = Animator.StringToHash("MoveX");
        _moveYHash = Animator.StringToHash("MoveY");
        _isFallingHash = Animator.StringToHash("IsFalling");
        
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

        //States
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        
        _playerInput = new PlayerInput();
        //Callback context for movement
        _playerInput.CharacterControls.Movement.started += OnMovementInput;
        _playerInput.CharacterControls.Movement.performed += OnMovementInput;
        _playerInput.CharacterControls.Movement.canceled += OnMovementInput;
        
        //Callback context for jump
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravityValue = (-2 * maxJumpHeight) / (timeToApex * timeToApex);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void Start()
    {
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateStates();
        updateStateName();
        //If should move and not following
        //or
        //following and jumping
        if ((shouldMove && !shouldFollow) || (shouldFollow && _isJumping))
        {
            _characterController.Move(_appliedMovement * Time.deltaTime);
            //HandleMovement();
        }
    }
    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        //If not allowed to be controlled
        if (shouldControl)
        {
            _currentMovementInput = ctx.ReadValue<Vector2>();
            
            _currentMovement.x = _currentMovementInput.x * speed;
            _currentMovement.z = _currentMovementInput.y * speed;
            
        }
        
        //Store last non-zero movement
        if (!_currentMovementInput.Equals(Vector2.zero))
        {
            _lastMovement.x = _currentMovementInput.x;
            _lastMovement.z = _currentMovementInput.y;
            //_lastMovement = _currentMovementInput;
        }
        
        _isMovementPressed = (_currentMovementInput.x != 0 || _currentMovementInput.y != 0); //Set true if x or y greater than 0
        
        roundedMovement.x = Mathf.Round(_currentMovementInput.x);
        roundedMovement.y = Mathf.Round(_currentMovementInput.y);
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (shouldControl)
        {
            _isJumpPressed = ctx.ReadValueAsButton();
            _requireNewJumpPress = false;
        }
    }

    //When this character is made active
    public void Activate()
    {
        //EnableControls();
        shouldControl = true;
        shouldFollow = false;
        
        //ResetMovement();
    }

    //When this character is made inactive
    public void Deactivate()
    {
        //DisableControls();
        shouldControl = false;
        shouldFollow = true;
        
        //Here, maybe do logic about following
        ResetMovement();
        
    }

    public void ResetMovement()
    {
        _currentMovementInput = Vector2.zero;
        _isMovementPressed = false;
        Debug.Log(name+" movement reset");
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

    private void updateStateName()
    {
        currentStateName = _currentState.ToString();
        if (_currentState.HasSubState())
        {
            currentSubStateName = _currentState.CurrentSubState.ToString();
        }
        else
        {
            currentSubStateName = "None";
        }
    }
}
