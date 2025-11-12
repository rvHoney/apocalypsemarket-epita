using UnityEngine;

public class MoveTmp : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(1f, 10f)]
    public float walkSpeed = 5f;
    
    [Range(1f, 15f)]
    public float runSpeed = 8f;
    
    [Header("Physics")]
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    
    [Header("Surface Detection")]
    public float slopeLimit = 45f;
    public float stepOffset = 0.3f;
    public bool useSlopeLimit = true;
    
    private InputMaster inputMaster;
    private CharacterController controller;
    private Vector2 moveInput;
    private bool isRunning;
    private bool isGrounded;
    
    // Variables pour le mouvement et les surfaces
    private Vector3 moveDirection;
    private Vector3 velocity;
    private float currentSpeed;
    private Vector3 lastMoveDirection;
    
    private void Awake()
    {
        // Initialiser le système d'input
        inputMaster = new InputMaster();
        
        // Obtenir le CharacterController
        controller = GetComponent<CharacterController>();
        
        // Configurer le CharacterController pour de meilleures interactions avec les surfaces
        if (controller != null)
        {
            controller.slopeLimit = slopeLimit;
            controller.stepOffset = stepOffset;
        }
    }
    
    private void Update()
    {
        HandleInput();
        CheckGrounded();
        HandleMovement();
    }
    
    private void HandleInput()
    {
        // Lire les inputs de mouvement
        moveInput = inputMaster.Player.Move.ReadValue<Vector2>();
        
        // Lire l'input de course
        isRunning = inputMaster.Player.Run.IsPressed();
    }
    
    private void CheckGrounded()
    {
        // Utiliser la détection automatique du CharacterController
        isGrounded = controller.isGrounded;
    }
    
    private void HandleMovement()
    {
        // Calculer la direction de mouvement
        moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        // Déterminer la vitesse selon l'état
        if (moveInput.magnitude > 0.1f)
        {
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            lastMoveDirection = moveDirection; // Garder la dernière direction pour les surfaces
        }
        else
        {
            currentSpeed = 0f;
        }
        
        // Appliquer le mouvement horizontal
        Vector3 horizontalMovement = moveDirection * currentSpeed;
        
        // Gérer la gravité et le saut
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Petite valeur pour maintenir au sol
        }
        
        velocity.y += gravity * Time.deltaTime;
        
        // Appliquer le mouvement avec CharacterController
        // Le CharacterController gère automatiquement les collisions avec les surfaces
        CollisionFlags collisionFlags = controller.Move((horizontalMovement + velocity) * Time.deltaTime);
        
        // Vérifier les collisions avec les surfaces
        HandleSurfaceCollisions(collisionFlags);
    }
    
    private void HandleSurfaceCollisions(CollisionFlags flags)
    {
        // Si on touche le sol, arrêter la vélocité verticale
        if ((flags & CollisionFlags.Below) != 0)
        {
            velocity.y = 0f;
        }
        
        // Si on touche un mur, ajuster le mouvement
        if ((flags & CollisionFlags.Sides) != 0)
        {
            // Le CharacterController gère automatiquement le glissement le long des murs
        }
        
        // Si on touche le plafond, arrêter la vélocité vers le haut
        if ((flags & CollisionFlags.Above) != 0)
        {
            velocity.y = 0f;
        }
    }
    
    // Méthode pour sauter (peut être appelée depuis l'extérieur)
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    
    // Méthodes publiques pour contrôler le mouvement
    public void SetWalkSpeed(float speed)
    {
        walkSpeed = speed;
    }
    
    public void SetRunSpeed(float speed)
    {
        runSpeed = speed;
    }
    
    public void SetJumpHeight(float height)
    {
        jumpHeight = height;
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public bool IsMoving()
    {
        return moveInput.magnitude > 0.1f;
    }
    
    public bool IsRunning()
    {
        return isRunning && IsMoving();
    }
    
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    // Méthodes pour bloquer/débloquer le mouvement
    public void LockMovement()
    {
        velocity = Vector3.zero;
        enabled = false;
    }
    
    public void UnlockMovement()
    {
        enabled = true;
    }
    
    private void OnEnable()
    {
        inputMaster?.Enable();
    }
    
    private void OnDisable()
    {
        inputMaster?.Disable();
    }
    
    // Debug : dessiner les informations de collision
    private void OnDrawGizmosSelected()
    {
        if (controller != null)
        {
            // Dessiner la capsule du CharacterController
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(transform.position + controller.center, 
                new Vector3(controller.radius * 2, controller.height, controller.radius * 2));
        }
    }
}
