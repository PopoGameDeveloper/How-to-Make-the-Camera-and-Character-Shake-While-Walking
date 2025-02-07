using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The movement speed of the player in units per second")]
    public float moveSpeed = 5f;

    [Tooltip("The sprint speed of the player in units per second")]
    public float sprintSpeed = 8f; // New variable for sprint speed

    [Header("Camera Bobbing")]
    [Tooltip("Speed of the camera bobbing effect during movement")]
    public float walkBobbingSpeed = 5f;
    
    [Tooltip("Intensity of the camera bobbing effect")]
    public float walkBobbingAmount = 0.0003f;

    [Header("Mouse Look")]
    [Tooltip("Sensitivity multiplier for mouse input")]
    public float mouseSensitivity = 100f;
    
    [Tooltip("Maximum vertical angle the camera can tilt (degrees)")]
    public float verticalLookLimit = 35f;

    [Header("Jumping")]
    [Tooltip("Initial upward velocity when jumping")]
    public float jumpSpeed = 5f;
    
    [Tooltip("Gravity force applied to the player")]
    public float gravity = -9.81f;

    // Reference to the CharacterController component
    private CharacterController controller;
    
    // Reference to the main camera's transform
    private Transform cameraTransform;
    
    // Original Y position of the camera
    private float defaultCameraY;
    
    // Timer for camera bobbing animation
    private float timer = 0f;
    
    // Current vertical rotation angle of the camera
    private float verticalRotation = 0f;
    
    // Vertical movement velocity (for jumping/gravity)
    private float verticalVelocity = 0f;

    /// <summary>
    /// Initialization method called once at startup
    /// </summary>
    void Start()
    {
        // Get required components
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        // Store initial camera position
        defaultCameraY = cameraTransform.localPosition.y;

        // Configure cursor state
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Main update loop called every frame
    /// </summary>
    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        UpdateCameraEffects();
    }

    /// <summary>
    /// Handles player movement including walking, sprinting, and jumping
    /// </summary>
    void HandleMovement()
    {
        // Check if player is touching ground
        bool isGrounded = controller.isGrounded;

        // Get keyboard input axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to player's orientation
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        // Check if the player is sprinting (Shift key is pressed)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Set movement speed based on whether the player is sprinting
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        moveDirection *= currentSpeed;

        // Gravity and jump handling
        if (isGrounded)
        {
            // Reset vertical velocity while grounded
            verticalVelocity = -0.5f;
            
            // Detect jump input
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpSpeed;
            }
        }
        else
        {
            // Apply gravity over time
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine horizontal and vertical movement
        moveDirection.y = verticalVelocity;
        
        // Apply movement to CharacterController
        controller.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    /// Handles camera rotation based on mouse input
    /// </summary>
    void HandleMouseLook()
    {
        // Get mouse input axis
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Calculate and clamp vertical camera rotation
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
        
        // Apply vertical rotation to camera
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    /// <summary>
    /// Updates camera effects including bobbing animation
    /// </summary>
    void UpdateCameraEffects()
{
    // Check if the player is moving
    bool isMoving = IsMoving();

    // Check if the player is sprinting
    bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    // Calculate the bobbing speed based on whether the player is sprinting
    float bobbingSpeed = isSprinting ? walkBobbingSpeed * 3 : walkBobbingSpeed;

   if (isMoving)
{
    // Animate camera position using sine wave
        timer += Time.deltaTime * bobbingSpeed; // Increase timer value by the elapsed time multiplied by bobbing speed
        float yOffset = Mathf.Sin(timer) * walkBobbingAmount; // Calculate the vertical offset for the camera using sine function and walk bobbing amount
        cameraTransform.localPosition = new Vector3(
        cameraTransform.localPosition.x, // X position of the camera
        defaultCameraY + yOffset, // Y position of the camera, adjusted by the vertical offset
        cameraTransform.localPosition.z // Z position of the camera
    );
}
    else
{
    // Smoothly reset camera position when not moving
        timer = 0f; // Reset the timer to 0
        cameraTransform.localPosition = Vector3.Lerp( //make smooth effect if the camera moves to target
        cameraTransform.localPosition, // Current camera position
        new Vector3(cameraTransform.localPosition.x, defaultCameraY, cameraTransform.localPosition.z), // Target camera position with only Y position adjusted
        Time.deltaTime * walkBobbingSpeed // Adjust the lerp speed based on walk bobbing speed
    );
}
}

    /// <summary>
    /// Checks if the player is currently moving
    /// </summary>
    /// <returns>True if receiving movement input</returns>
    bool IsMoving()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }
}