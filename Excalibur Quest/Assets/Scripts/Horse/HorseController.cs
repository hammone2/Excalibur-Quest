using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class HorseController : MonoBehaviour
{
    [Header("Mounting")]
    public Transform mountPoint;
    public Transform dismountPoint;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float gravity = -25f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string turnParameter = "Turn";

    private CharacterController controller;
    private InputSystem_Actions inputActions;
    private PlayerController rider;

    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
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
        ApplyGravity();

        if (rider == null)
            return;

        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        float speed = inputActions.Player.Sprint.IsPressed()
            ? sprintSpeed
            : walkSpeed;

        transform.Rotate(
            Vector3.up,
            input.x * turnSpeed * Time.deltaTime
        );

        Vector3 movement =
            transform.forward * input.y * speed;

        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        animator.SetFloat(
            speedParameter,
            Mathf.Abs(input.y),
            0.15f,
            Time.deltaTime
        );

        animator.SetFloat(
            turnParameter,
            input.x,
            0.15f,
            Time.deltaTime
        );
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        if (rider == null)
        {
            controller.Move(
                Vector3.up * verticalVelocity * Time.deltaTime
            );
        }
    }

    public void SetRider(PlayerController player)
    {
        rider = player;
    }

    public void RemoveRider()
    {
        rider = null;

        animator.SetFloat(speedParameter, 0f);
        animator.SetFloat(turnParameter, 0f);
    }
}

