using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HorseController : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public float rotationSpeed = 100f;
    public float gravity = -20f;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;
    private float verticalVelocity;
    private bool beingRidden;

    public bool BeingRidden => beingRidden;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Update()
    {
        if (!beingRidden)
        {
            ApplyGravity();
            return;
        }

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        float forwardInput = input.y;
        float turnInput = input.x;

        float speed = Keyboard.current != null &&
                      Keyboard.current.leftShiftKey.isPressed
            ? runSpeed
            : walkSpeed;

        transform.Rotate(
            Vector3.up,
            turnInput * rotationSpeed * Time.deltaTime
        );

        Vector3 movement =
            transform.forward * forwardInput * speed;

        ApplyGravity();

        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        animator.SetFloat(
            "Speed",
            Mathf.Abs(forwardInput),
            0.15f,
            Time.deltaTime
        );

        animator.SetFloat(
            "Turn",
            turnInput,
            0.15f,
            Time.deltaTime
        );
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
    }

    public void SetRidden(bool value)
    {
        beingRidden = value;

        if (!value)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("Turn", 0f);
        }
    }
}
