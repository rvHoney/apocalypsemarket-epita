using UnityEngine;

//script created by Antoine(Icescream) bruh

public class Look : MonoBehaviour
{
    #region Singleton
    public static Look instance;
    #endregion

    InputMaster inputMaster;
    Vector2 axis;

    [Range(0f, 15f)] public float mouseSens = 7f;
    private float savedSensitivity;
    [SerializeField] private Transform player;        

    public bool invertSensitivity = false;

    Quaternion prevRotation;
    bool locked;

    private Camera cam;
    [SerializeField]float maxZoomFov = 45f;
    float fov = 60f;

    private void Awake()
    {
        cam = Camera.main;
        fov = cam.fieldOfView;
        if (!instance)
        {
            instance = this;
        }

        inputMaster = new InputMaster();
        //inputMaster.Player.Look.performed += ctx => axis = ctx.ReadValue<Vector2>();
    }

    private int invertSens()
    {
        if (invertSensitivity)
            return -1;
        else return 1;
    }

    private float _xRootation = 0f;

    [HideInInspector] public bool sensitivityHasChanged = false;

    private void Start()
    {
        savedSensitivity = mouseSens;
        ShowCursor(false);
    }

    private void LateUpdate()
    {
        axis = inputMaster.Player.Look.ReadValue<Vector2>();

        float mouseX = axis.x * mouseSens * Time.fixedDeltaTime;
        float mouseY = axis.y * mouseSens * Time.fixedDeltaTime * invertSens();

        _xRootation -= mouseY;
        _xRootation = Mathf.Clamp(_xRootation, -90f, 90f);

        if (!locked)
        {
            transform.localRotation = Quaternion.Euler(_xRootation, 0f, 0f);
            player.Rotate(Vector3.up * mouseX);
        }
        
    }

    public void LockCameraRotation()
    {
        mouseSens = 0;
        locked = true;
    }

    public void SlowCameraRotation()
    {
        float slowedSensitivity = savedSensitivity / 30;
        mouseSens = slowedSensitivity;
    }
    public void resetSensitivity()
    {
        locked = false;
        mouseSens = savedSensitivity;
    }
    public void saveRotation()
    {
        prevRotation = transform.rotation;
    }
    public void resetRotation()
    {
        
        transform.rotation = prevRotation;
    }

    public void ChangeSensitivity(float newSens)
    {
        savedSensitivity = newSens;
        mouseSens = savedSensitivity;
    }
    public void OverrideCameraPosition(Transform target,bool smooth , bool lockCameraMovement, bool lockPlayerMovement)
    {
        if(lockCameraMovement)
            LockCameraRotation();
        if (lockPlayerMovement)
            PlayerMove.instance.LockPlayerInPlace();
        transform.position = target.position;
        locked = true;
        transform.rotation = target.rotation;
        
    }

    public void ZoomIn(float delta, float speed)
    {        
        cam.fieldOfView -= delta * speed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, maxZoomFov, fov);
    }
    public void ResetFOV()
    {
        cam.fieldOfView = fov;
    }

    public void ShowCursor(bool show)
    {
        if (show) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = show;
    }

    private void OnEnable()
    {
        inputMaster.Enable();
    }
}
