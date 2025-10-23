using UnityEngine;

// Created by Antoine(Icescream)
public class Look : MonoBehaviour
{
    public static Look instance;

    private InputMaster inputMaster;
    private Vector2 rawInput;    
    private Vector2 smoothedInput;  
    private Vector2 inputVelocity;   

    [Header("Sensitivity")]
    [Range(0f, 30f)] public float mouseSens = 7f;
    private float savedSensitivity;
    public bool invertSensitivity = false;

    [Header("Smoothing")]
    [Range(0f, 25f)] public float smoothSpeed = 20f;
    [SerializeField] private bool enableSmoothing = true;

    [Header("References")]
    [SerializeField] private Transform player;

    private Camera cam;
    private float xRotation = 0f;
    private float defaultFov;
    [SerializeField] private float maxZoomFov = 45f;

    private bool locked;

    private void Awake()
    {
        if (!instance) instance = this;
        inputMaster = new InputMaster();
    }

    private void Start()
    {
        cam = Camera.main;
        defaultFov = cam.fieldOfView;
        savedSensitivity = mouseSens;
        ShowCursor(false);
    }

    private void Update()
    {
        HandleLook();
    }

    private void HandleLook()
    {
        rawInput = inputMaster.Player.Look.ReadValue<Vector2>();

        // smoothing 
        Vector2 finalInput = enableSmoothing
            ? Vector2.Lerp(smoothedInput, rawInput, smoothSpeed * Time.deltaTime)
            : rawInput;

        smoothedInput = finalInput;

        float mouseX = finalInput.x * mouseSens * Time.deltaTime;
        float mouseY = finalInput.y * mouseSens * Time.deltaTime * (invertSensitivity ? -1f : 1f);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (!locked)
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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
        mouseSens = savedSensitivity / 30f;
    }

    public void ResetSensitivity()
    {
        locked = false;
        mouseSens = savedSensitivity;
    }

    public void ZoomIn(float delta, float speed)
    {
        cam.fieldOfView -= delta * speed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, maxZoomFov, defaultFov);
    }

    public void ResetFOV()
    {
        cam.fieldOfView = defaultFov;
    }

    public void ShowCursor(bool show)
    {
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }

    private void OnEnable() => inputMaster.Enable();
}
