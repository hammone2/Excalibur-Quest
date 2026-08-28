using UnityEngine;

public class NewTPController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 2f;
    public float SprintSpeed = 5.3f;
    public float Acceleration = 10f;
    public float RotationSmoothTime = 0.12f;

    [Header("Jump")]
    public float JumpHeight = 1.2f;
    public float Gravity = -15f;

    [Header("Ground")]
    public float GroundedRadius = 0.28f;
    public float GroundedOffset = -0.14f;
    public LayerMask GroundLayers;
    public bool Grounded;

    [Header("Camera")]
    public Transform CameraTarget;
    public float TopClamp = 70f;
    public float BottomClamp = -30f;
    public bool LockCamera = false;
    private CharacterController controller;
    private Camera mainCamera;


    private InputSystem_Actions input;
    private float speed;
    private float verticalVelocity;
    private float rotationVelocity;

    private float cameraYaw;
    private float cameraPitch;

    private const float TerminalVelocity = 53f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        input = new InputSystem_Actions();

        cameraYaw = CameraTarget.eulerAngles.y;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        CheckGround();
        Jump();
        Move();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    private void CheckGround()
    {
        Vector3 position =
            transform.position +
            Vector3.down * GroundedOffset;

        Grounded = Physics.CheckSphere(
            position,
            GroundedRadius,
            GroundLayers,
            QueryTriggerInteraction.Ignore
        );
    }

    private void Move()
    {
        Vector2 inputVector =
            input.Player.Move.ReadValue<Vector2>();

        Vector3 direction = new Vector3(
            inputVector.x,
            0f,
            inputVector.y
        );

        direction = Vector3.ClampMagnitude(direction, 1f);

        float targetSpeed =
            input.Player.Sprint.IsPressed()
                ? SprintSpeed
                : MoveSpeed;

        if (direction == Vector3.zero)
            targetSpeed = 0f;

        speed = Mathf.MoveTowards(
            speed,
            targetSpeed,
            Acceleration * Time.deltaTime
        );

        if (direction != Vector3.zero)
        {
            float targetAngle =
                Mathf.Atan2(direction.x, direction.z) *
                Mathf.Rad2Deg +
                mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                RotationSmoothTime
            );

            transform.rotation =
                Quaternion.Euler(0f, rotation, 0f);

            Vector3 moveDirection =
                Quaternion.Euler(0f, targetAngle, 0f) *
                Vector3.forward;

            controller.Move(
                moveDirection * speed * Time.deltaTime +
                Vector3.up * verticalVelocity * Time.deltaTime
            );
        }
        else
        {
            controller.Move(
                Vector3.up *
                verticalVelocity *
                Time.deltaTime
            );
        }
    }

    private void Jump()
    {
        if (Grounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (input.Player.Jump.WasPressedThisFrame())
            {
                verticalVelocity =
                    Mathf.Sqrt(
                        JumpHeight * -2f * Gravity
                    );
            }
        }

        if (verticalVelocity < TerminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void CameraLook()
    {
        if (LockCamera)
            return;

        Vector2 look =
            input.Player.Look.ReadValue<Vector2>();

        cameraYaw += look.x * 0.1f;
        cameraPitch -= look.y * 0.1f;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            BottomClamp,
            TopClamp
        );

        CameraTarget.rotation =
            Quaternion.Euler(
                cameraPitch,
                cameraYaw,
                0f
            );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Grounded
            ? Color.green
            : Color.red;

        Gizmos.DrawSphere(
            transform.position +
            Vector3.down * GroundedOffset,
            GroundedRadius
        );
    }
}
