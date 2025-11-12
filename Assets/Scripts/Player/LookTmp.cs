using UnityEngine;

public class LookTmp : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(1f, 20f)]
    public float mouseSensitivity = 5f;
    
    [Header("Player Reference")]
    public Transform playerBody;
    
    [Header("Camera Limits")]
    public float maxLookAngle = 80f;
    
    private float xRotation = 0f;
    private InputMaster inputMaster;
    
    private void Awake()
    {
        // Initialiser le système d'input
        inputMaster = new InputMaster();
        
        // Verrouiller le curseur au centre de l'écran
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Start()
    {
        // Si aucun playerBody n'est assigné, essayer de le trouver automatiquement
        if (playerBody == null)
        {
            playerBody = transform.parent;
        }
    }
    
    private void Update()
    {
        HandleMouseLook();
    }
    
    private void HandleMouseLook()
    {
        // Lire l'input de la souris
        Vector2 mouseInput = inputMaster.Player.Look.ReadValue<Vector2>();
        
        // Calculer la rotation horizontale (Y) pour le joueur
        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;
        
        // Rotation horizontale du joueur
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
        
        // Rotation verticale de la caméra
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        // Appliquer la rotation verticale à la caméra
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    private void OnEnable()
    {
        inputMaster?.Enable();
    }
    
    private void OnDisable()
    {
        inputMaster?.Disable();
    }
    
    // Méthodes publiques pour contrôler la caméra
    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
    
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void ResetCameraRotation()
    {
        xRotation = 0f;
        transform.localRotation = Quaternion.identity;
    }
}
