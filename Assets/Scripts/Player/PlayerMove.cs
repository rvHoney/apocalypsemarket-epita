using UnityEngine;
using UnityEngine.InputSystem;

//Script made by Antoine(Icescream)

public class PlayerMove : MonoBehaviour
{
    #region Singleton
    public static PlayerMove instance;
    #endregion
    private Vector2 axis;
    bool pressShift;
    bool pressControl;
    InputMaster inputMaster;

    [SerializeField] [Range(1f, 10f)] private float walkSpeed = 3f;
    [SerializeField] [Range(1f, 15f)] private float runSpeed = 5f;
    [SerializeField] [Range(.01f, 10.00f)] private float crouchSpeed = 1.75f;
    public float stepOffset = 0.3f;

    private float _currentSpeed = 1f;
    public float speedMultiplier = 1f;

    [Tooltip("The height that the player will crouch")]
    [SerializeField] [Range(.01f, 2f)] private float crouchHeight = 1f;
    float normalHeight;

    private Rigidbody charRB;
    private CapsuleCollider playerCollider;

    private float heightDelta;

    [Tooltip("The graph corresponding to the easing of the crouching animation")]
    public AnimationCurve cameraSmoothing;
    [Tooltip("This value will change the speed of the animation (0=pause, 1=normal speed, 2=double speed")]
    public float animationSpeed;
    private Vector3 cameraDesiredPosition;
    private Vector3 cameraInitialPosition;
    [SerializeField] private Transform playerCamera;
    private float step;

    Vector3 moveDir;

    public bool isGrounded;
    public bool isMoving;

    bool locked;

    public enum playerMovementStatus
    {
        walking = 0,
        running = 1,
        crouching = 2,
        idle = 3
    };

    public playerMovementStatus playerStatus;

    void Awake()
    {        
        instance = this;

        inputMaster = new InputMaster();
        // inputMaster.Player.Move.performed += ctx => axis = ctx.ReadValue<Vector2>();
        inputMaster.Player.Run.performed += ctx => pressShift = true;
        inputMaster.Player.Run.canceled += ctx => pressShift = false;
        inputMaster.Player.Crouch.performed += ctx => pressControl = !pressControl;
    }

    void Start()
    {
        charRB = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        normalHeight = playerCollider.height;
        cameraInitialPosition = playerCamera.transform.localPosition;

        float distanceToGround = GetComponent<Collider>().bounds.extents.y;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround + 0.2f);    
    }
    
    void FixedUpdate()
    {     
        axis = inputMaster.Player.Move.ReadValue<Vector2>();
        if(!locked)
            PlayerMovement();
    }

    private void PlayerMovement()
    {
        
        SetMovementSpeed();        
        crouch();

        _currentSpeed *= speedMultiplier;
        float vertInput = axis.y * _currentSpeed;
        float horizInput = axis.x * _currentSpeed;
        
        moveDir = new Vector3(horizInput , charRB.linearVelocity.y, vertInput);
        moveDir = transform.TransformDirection(moveDir);

        charRB.linearVelocity = moveDir;     
    }

    private void SetMovementSpeed()
    {
        //if player standing still
        if ((axis.x == 0 && axis.y == 0)) {
            _currentSpeed = 0f;
            playerStatus = playerMovementStatus.idle;
            isMoving = false;
        }
        //if player moving
        else if ((axis.x != 0 || axis.y != 0))
        {
            isMoving = true;
            //if player running
            if (pressShift && !pressControl)
            {
                _currentSpeed = runSpeed;
                playerStatus = playerMovementStatus.running;
            }
            //if player crouching
            else if (pressControl)
            {
                _currentSpeed = crouchSpeed;
                playerStatus = playerMovementStatus.crouching;
            }
            //if player is walking
            else
            {
                _currentSpeed = walkSpeed;
                playerStatus = playerMovementStatus.walking;
            }
            //if player is stuck on a wall
            if(charRB.linearVelocity.magnitude <= 0.3f)
            {
                playerStatus = playerMovementStatus.idle;
            }

            //if player is moving backward or sideways
            if(axis.y <= 0)
            {
                _currentSpeed /= 1.5f;
            }
        }        
    }
    private void crouch()
    {
        step = Mathf.Clamp(step, 0f, 1f);
        heightDelta = crouchHeight - normalHeight;
        cameraDesiredPosition = new Vector3(cameraInitialPosition.x, cameraInitialPosition.y + heightDelta, cameraInitialPosition.z);
        if (pressControl)
        {
            playerCollider.height = crouchHeight;
            playerCollider.center = new Vector3(0, heightDelta / 2, 0);
            //playerCamera.transform.localPosition = new Vector3(0, cameraDesiredPosition, 0);
            playerCamera.transform.localPosition = Vector3.Lerp(cameraInitialPosition, cameraDesiredPosition, cameraSmoothing.Evaluate(step));
            step -= Time.deltaTime * animationSpeed;
        }
        else
        {
            playerCollider.height = normalHeight;
            playerCollider.center = new Vector3(0, 0, 0);
            //playerCamera.transform.localPosition = new Vector3(0, cameraInitialPosition, 0);
            step += Time.deltaTime * animationSpeed;
            if(cameraInitialPosition.y - playerCamera.localPosition.y > 0.01f)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(cameraInitialPosition, cameraDesiredPosition, cameraSmoothing.Evaluate(step));
            }
            else
            {
                playerCamera.transform.localPosition = cameraInitialPosition;
            }
        }
    }

    public void LockPlayerInPlace()
    {
        locked = true;
        playerStatus = playerMovementStatus.idle;
    }
    public void UnlockPlayer()
    {
        locked = false;
    }

    public void ChangeMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
    private void OnEnable()
    {
        inputMaster.Enable();
    }
    private void OnDisable()
    {
        inputMaster.Disable();
    }
}
