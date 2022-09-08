using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    //PlayerStateController _playerStateController;
    //Movement variables
    [Header("Movement Variables")]
    public bool shouldMove = true;
    public bool isMovementPressed;
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
    public float initialJumpVelocity;
    public float maxJumpHeight = 1.0f;
    public float maxJumpTime = 0.75f;
    public float maxFallVelocity = -10.0f;
    private float _fallMultiplier = 2.0f;
    
    //Interaction variables
    [Header("Interactions")] 
    public bool shouldInteract = true;
    public AbstractInteractable interactable;

    //Action variables
    [Header("Action")] 
    public CharacterAction characterAction;
    public bool shouldAction;

    //Party variables
    [Header("Party and Swapping")] 
    public bool shouldFollow;
    public FollowPlayer followPlayer;
    
    //Animator
    [Header("Animation")]
    public Animator animator;

    public Vector2 roundedMovement;
    public bool shouldAnimate = true;
    public bool shouldUpdateMoveAnims = true;
    //Variables to store animation parameter IDs

    [Header("Components")] 
    private CharacterController _characterController;
    
    //Instance of PlayerInput
    [Header("Player Input")] 
    public bool shouldControl; 
    PlayerInput _playerInput;

    //Storing player input values
    [Header("Input Vectors")]
    private Vector2 _currentMovementInput; 
    private Vector3 _currentMovement;
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
        get => _currentState;
        set => _currentState = value;
    }
    public Animator Animator => animator;

    public CharacterController CharacterController => _characterController;
    public int IsMovingHash { get; private set; }
    public int IsJumpingHash { get; private set; }
    public int MoveXHash { get; private set; }
    public int MoveYHash { get; private set; }
    public int IsFallingHash { get; private set; }
    public bool RequireNewJumpPress { get; set; }
    public bool IsJumping { get; set; }
    public bool IsJumpPressed { get; set; }
    public float Gravity => gravityValue;
    public float MaxFallVelocity => maxFallVelocity;
    public bool IsMovementPressed => isMovementPressed;
    public float CurrentMovementX { get => _currentMovement.x;
        set => _currentMovement.x = value;
    }
    public float CurrentMovementY { get => _currentMovement.y;
        set => _currentMovement.y = value;
    }
    public float CurrentMovementZ { get => _currentMovement.z;
        set => _currentMovement.z = value;
    }
    public float AppliedMovementX { get => _appliedMovement.x;
        set => _appliedMovement.x = value;
    }
    public float AppliedMovementY { get => _appliedMovement.y;
        set => _appliedMovement.y = value;
    }
    public float AppliedMovementZ { get => _appliedMovement.z;
        set => _appliedMovement.z = value;
    }
    public float LastMovementX { get => _lastMovement.x;
        set => _lastMovement.x = value;
    }
    public float LastMovementY { get => _lastMovement.z;
        set => _lastMovement.z = value;
    }
    public Vector3 CurrentMovement { get => _currentMovement;
        set => _currentMovement = value;
    } 
    public Vector3 AppliedMovement { get => _appliedMovement;
        set => _appliedMovement = value;
    }
    public Vector3 LastMovement { get => _lastMovement;
        set => _lastMovement = value;
    }
    public float GroundedGravity => _groundedGravity;
    public float FallMultiplier => _fallMultiplier;
    public Vector2 CurrentMovementInput { get => _currentMovementInput; set => _currentMovementInput = value; }
    public bool IsActionPressed { get; set; }

    public int IsPerformingActionHash { get; private set; }
    private void Awake()
    {
        //Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        IsMovingHash = Animator.StringToHash("IsMoving");
        IsJumpingHash = Animator.StringToHash("IsJumping");
        MoveXHash = Animator.StringToHash("MoveX");
        MoveYHash = Animator.StringToHash("MoveY");
        IsFallingHash = Animator.StringToHash("IsFalling");
        IsPerformingActionHash = Animator.StringToHash("IsAttack");
        
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

        //States
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        
        _playerInput = new PlayerInput();
        //Input controlled by party manager

        //AI navigation
        followPlayer = GetComponent<FollowPlayer>();
        if (followPlayer != null)
        {
            followPlayer.OnLinkStart += HandleLinkStart;
            followPlayer.OnLinkEnd += HandleLinkEnd;
        }
        
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
        UpdateMovement();
    }
    

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateStates();
        UpdateStateName();
        
        /*
         * If should move and not following
         */
        if (shouldMove && !shouldFollow)
        {
            UpdateMovement();
        }
    }
    
    public void UpdateMovement()
    {
        _characterController.Move(AppliedMovement * Time.deltaTime);
    }

    public void OnMovement(Vector2 movementInput)
    {
        _currentMovementInput = movementInput;
        CurrentMovementX = movementInput.x * speed;
        CurrentMovementZ = movementInput.y * speed;

        //Store last non-zero movement
        if (!movementInput.Equals(Vector2.zero))
        {
            LastMovementX = movementInput.x;
            LastMovementY = movementInput.y;
            //_lastMovement = _currentMovementInput;
        }

        roundedMovement.x = Mathf.Round(movementInput.x);
        roundedMovement.y = Mathf.Round(movementInput.y);
        
        isMovementPressed = (movementInput.x != 0 || movementInput.y != 0); //Set true if x or y greater than 0
    }

    public void FreezeCurrentMovement()
    {
        CurrentMovementX = 0;
        CurrentMovementZ = 0;
    }

    //When this character is made active
    public void Activate()
    {
        shouldControl = true;
        shouldFollow = false;
    }

    //When this character is made inactive
    public void Deactivate()
    {
        shouldControl = false;
        shouldFollow = true;

        FreezeCurrentMovement();
    }

    public void FollowTarget(Transform targetTransform)
    {
        if (followPlayer != null)
        {
            followPlayer.EnablePathfinding(targetTransform);
        }
    }

    public void DisablePathfinding()
    {
        if (followPlayer != null)
        {
            followPlayer.DisablePathfinding();
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

    private void UpdateStateName()
    {
        currentStateName = _currentState.ToString();
        currentSubStateName = _currentState.HasSubState() ? _currentState.CurrentSubState.ToString() : "None";
    }

    public void UpdateMoveAnims(Vector2 moveAnimsVector)
    {
        roundedMovement.x = Mathf.Round(moveAnimsVector.x);
        roundedMovement.y = Mathf.Round(moveAnimsVector.y);
        
        //Store last non-zero movement
        if (!roundedMovement.Equals(Vector2.zero))
        {
            LastMovementX = moveAnimsVector.x;
            LastMovementY = moveAnimsVector.y;
        }
        
        isMovementPressed = (moveAnimsVector.x != 0 || moveAnimsVector.y != 0); //Set true if x or y not equal to 0
    }

    private void HandleLinkStart()
    {
        IsJumpPressed = true;
        RequireNewJumpPress = false;
    }

    private void HandleLinkEnd()
    {
        IsJumpPressed = false;
    }
}
