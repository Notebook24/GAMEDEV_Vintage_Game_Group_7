using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // ===== EDIT THESE IN CODE (not shown in the Inspector) =====
    // Movement
    public const float WalkSpeed = 4f;
    public const float SprintMultiplier = 1.8f;
    public const float SprintSpeed = WalkSpeed * SprintMultiplier; // 7.2
    const float Gravity = -20f;

    // Mouse look
    const float MouseSensitivity = 2f;
    const float MaxLookAngle = 90f;

    // Stamina
    public const float MaxStamina = 100f;
    const float DrainRate = 20f;          // per second while sprinting
    const float RegenRate = 8f;          // per second while not sprinting
    const float RegenDelay = 1f;          // pause before regen starts
    const float MinStaminaToSprint = 10f; // must recover to this after hitting 0
    // ===========================================================

    // SWE 1 reads these for the UI
    public float CurrentStamina { get; private set; }
    public float StaminaNormalized => CurrentStamina / MaxStamina;

    CharacterController controller;
    Transform cameraTransform;
    float pitch;
    float verticalVelocity;
    float regenTimer;
    bool exhausted;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform; // no Inspector slot needed
        CurrentStamina = MaxStamina;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mx = Input.GetAxis("Mouse X") * MouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * MouseSensitivity;

        transform.Rotate(Vector3.up * mx);

        pitch = Mathf.Clamp(pitch - my, -MaxLookAngle, MaxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.ClampMagnitude(new Vector3(x, 0, z), 1f);
        bool isMoving = input.sqrMagnitude > 0.01f;

        // Sprint logic
        if (CurrentStamina <= 0f) exhausted = true;
        if (exhausted && CurrentStamina >= MinStaminaToSprint) exhausted = false;

        bool sprinting = Input.GetKey(KeyCode.LeftShift) && isMoving && !exhausted;

        if (sprinting)
        {
            CurrentStamina = Mathf.Max(CurrentStamina - DrainRate * Time.deltaTime, 0f);
            regenTimer = RegenDelay;
        }
        else
        {
            if (regenTimer > 0f) regenTimer -= Time.deltaTime;
            else CurrentStamina = Mathf.Min(CurrentStamina + RegenRate * Time.deltaTime, MaxStamina);
        }

        float speed = sprinting ? SprintSpeed : WalkSpeed;
        Vector3 move = transform.TransformDirection(input) * speed;

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += Gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}