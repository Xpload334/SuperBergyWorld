using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerStateController _playerStateController;
    //Movement variables
    [Header("Movement Variables")]
    public bool shouldMove = true;
    bool _isMovementPressed;
    public float speed = 5.0f;
    
    //Gravity variables
    [Header("Gravity Variables")]
    public bool hasGravity = true;
    public float gravityValue = -10f;
    float groundedGravity = -0.05f;
    
    //Jumping variables
    [Header("Jumping")]
    public bool shouldJump = true;
    public bool shouldAdjustJump = true;
    public bool isJumping;
    bool _isJumpPressed;
    public float initialJumpVelocity;
    public float maxJumpHeight = 1.0f;
    public float maxJumpTime = 0.75f;
    float _fallMultiplier = 2.0f;
    
    //Animator
    [Header("Animation")]
    public Animator animator;
    public bool shouldAnimate = true;
    //Variables to store animation parameter IDs
    private int _isMovingHash;
    private int _isJumpingHash;
    private int _moveXHash;
    private int _moveYHash;
    
    //public Collider collider;
    //public CharacterMove3D characterMove;
    [Header("Components")]
    public CharacterController characterController;
    
    //Instance of PlayerInput
    [Header("Player Input")] 
    public bool shouldControl; 
    PlayerInput _playerInput;

    //Storing player input values
    [Header("Input Vectors")]
    public Vector2 currentMovementInput; 
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    Vector3 _lastMovement;
    
    private void Awake()
    {
        //Animator
        _isMovingHash = Animator.StringToHash("IsMoving");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _moveXHash = Animator.StringToHash("MoveX");
        _moveYHash = Animator.StringToHash("MoveY");
        
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
        
        if (_playerStateController == null)
        {
            _playerStateController = GetComponent<PlayerStateController>();
        }

        _playerInput = new PlayerInput();
        //Callback context for movement
        _playerInput.CharacterControls.Movement.started += OnMovementInput;
        _playerInput.CharacterControls.Movement.performed += OnMovementInput;
        _playerInput.CharacterControls.Movement.canceled += OnMovementInput;
        
        //Callback context for jump
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        SetupJumpVariables();
        SetupPlayerStates();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravityValue = (-2 * maxJumpHeight) / (timeToApex * timeToApex);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void SetupPlayerStates()
    {
        _playerStateController.setIdleMovementState();
        if (shouldControl)
        {
            _playerStateController.setActiveState();
        }
        else
        {
            _playerStateController.setInactiveState();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldAnimate)
        {
            HandleAnimation();
        }
        
        if (shouldMove)
        {
            HandleMovement();
        }

        if (hasGravity)
        {
            HandleGravity();
        }

        if (shouldJump)
        {
            //tempJump();
            HandleJump();
        }
        
    }

    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        //If not allowed to be controlled
        if (!shouldControl)
        {
            
        }
        
        currentMovementInput = ctx.ReadValue<Vector2>();
        _currentMovement.x = currentMovementInput.x * speed;
        _currentMovement.z = currentMovementInput.y * speed;
            
        //Store last non-zero movement
        if (!currentMovementInput.Equals(Vector2.zero)) 
        {
            _lastMovement = _currentMovement;
        }
            
        //Debug.Log(currentMovement);
        _isMovementPressed = (currentMovementInput.x != 0 || currentMovementInput.y != 0); //Set true if x or y greater than 0
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        _isJumpPressed = ctx.ReadValueAsButton();
    }
    
    void HandleMovement()
    {
        _appliedMovement.x = _currentMovement.x;
        _appliedMovement.z = _currentMovement.z;
        characterController.Move(_appliedMovement * Time.deltaTime);
        //Potentially put all the actual movement on another script, such that the input receiver can move multiple characters
        //characterMove.move(currentMovement);

        //!characterController.isGrounded &&
        if(isJumping)
        {
            //Set jumping
            _playerStateController.setJumpingMovementState();
        }
        else
        {
            if (_isMovementPressed)
            {
                //Set walk
                _playerStateController.setWalkMovementState();
            }
            else
            {
                //Set idle
                _playerStateController.setIdleMovementState();
            }
        }
        
    }

    void HandleGravity()
    {
        bool isFalling;
        if (shouldAdjustJump)
        {
            isFalling = _currentMovement.y <= 0.0f || !_isJumpPressed;
        }
        else
        {
            isFalling = _currentMovement.y <= 0.0f;
        }
        
        //Apply correct gravity if the player is grounded or not
        if (characterController.isGrounded)
        {
            //Handle grounded animation
            animator.SetBool(_isJumpingHash, false);
            _currentMovement.y = groundedGravity;
            _appliedMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y += (gravityValue * _fallMultiplier * Time.deltaTime);
            _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y += (gravityValue * Time.deltaTime);
            _appliedMovement.y = (previousYVelocity + _currentMovement.y) * 0.5f;
        }
    }

    void HandleJump()
    {
        if (!isJumping && characterController.isGrounded && _isJumpPressed)
        {
            isJumping = true;
            //animator.SetBool(isJumpingHash, true);
            //isJumpAnimating = true;
            _currentMovement.y = initialJumpVelocity;
            _appliedMovement.y = initialJumpVelocity;
            
        } 
        else if (isJumping && characterController.isGrounded && !_isJumpPressed)
        {
            isJumping = false;
        }
    }

    void HandleAnimation()
    {
        bool isMoving = animator.GetBool(_isMovingHash); //Animator hash
        //bool isJumping;

        //Movement animations
        if (_isMovementPressed)
        {
            if(!isMoving) animator.SetBool(_isMovingHash, true);
            animator.SetFloat(_moveXHash, _currentMovement.x);
            animator.SetFloat(_moveYHash, _currentMovement.z);
        }
        //Idle animations
        else if (!_isMovementPressed)
        {
            if(isMoving) animator.SetBool(_isMovingHash, false);
            animator.SetFloat(_moveXHash, _lastMovement.x);
            animator.SetFloat(_moveYHash, _lastMovement.z);
        }
    }

    public void SetActiveState()
    {
        shouldControl = true;
        _playerStateController.setActiveState();
    }

    public void SetInactiveState()
    {
        shouldControl = false;
        _playerStateController.setInactiveState();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
