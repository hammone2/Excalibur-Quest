using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -25f;

    [Header("Ground")]
    [SerializeField] private float groundedForce = -2f;

    private CharacterController controller;
    private InputSystem_Actions inputActions;
    private Transform cameraTransform;

    private Vector3 velocity;
    private float currentSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new InputSystem_Actions();

        //Lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    private void Start()
    {
        if (Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }


    private void Update()
    {
        HandleMovement();
        HandleGravity();
        HandleJump();
    }

    private void HandleMovement()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 inputDirection = new Vector3(
            input.x,
            0f,
            input.y
        );

        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        if (cameraTransform == null)
            return;

        // Camera-relative movement.
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * inputDirection.z +
            cameraRight * inputDirection.x;

        bool sprinting =
            inputActions.Player.Sprint.IsPressed();

        float targetSpeed = sprinting
            ? sprintSpeed
            : moveSpeed;

        if (inputDirection.sqrMagnitude < 0.01f)
        {
            targetSpeed = 0f;
        }

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.deltaTime
        );

        controller.Move(
            moveDirection * currentSpeed * Time.deltaTime
        );

        // Rotate toward movement direction.
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleJump()
    {
        if (!controller.isGrounded)
            return;

        if (inputActions.Player.Jump.WasPressedThisFrame())
        {
            velocity.y =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = groundedForce;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }
}