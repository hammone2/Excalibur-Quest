using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -25f;
    private bool canDoubleJump = false;

    [Header("Ground")]
    [SerializeField] private float groundedForce = -2f;

    [Header("Camera Settings")]
    public CinemachineCamera followCamera;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private CinemachineCamera deathCamera;
    private CinemachineBrain brain;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    public bool LockCameraPosition = false;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Header("Look")]
    [SerializeField] private Transform aimCameraTarget;
    [SerializeField] private Transform followCameraTarget;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float controllerSensitivity = 100f;

    [Header("Aim Animation")]
    [SerializeField] private int aimLayerIndex = 1;
    [SerializeField] private float aimLayerBlendSpeed = 10f;
    [SerializeField] private float aimPitchBlendSpeed = 10f;
    [SerializeField] private float aimDownLimit = -45f;
    [SerializeField] private float aimUpLimit = 45f;
    private float currentAimLayerWeight;

    [Header("Weapon System")]
    public WeaponController weaponController;
    [SerializeField] private float attackDuration = 0.35f;
    [SerializeField] private float attackForwardDistance = 1.0f;
    public Animator animator;
    private float attackTimer;
    private Vector3 attackDirection;
    private Vector3 attackStartPosition;

    [Header("Health System")]
    public HealthComponent healthComponent;
    public ArmorComponent armorComponent;

    private CharacterController controller;
    private InputSystem_Actions inputActions;
    private Transform cameraTransform;

    private Vector3 velocity;
    private float currentSpeed;
    private bool aiming = false;
    private bool isFreelookEnabled = true;
    private bool previousAiming = false;
    private float cameraPitch;

    public bool HasBlendFinished => isFreelookEnabled;


    private enum PlayerState
    {
        Normal,
        Dashing,
        Attacking,
        Dead
    }
    private PlayerState currentState = PlayerState.Normal;


    public static PlayerController Instance { get; private set; }



    #region Start & Awake
    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new InputSystem_Actions();

        //Lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            brain = cameraTransform.GetComponent<CinemachineBrain>();
            cinemachineTargetYaw = followCameraTarget.rotation.eulerAngles.y;
        }

        Healthbar healthbar = HUDManager.Instance.healthBar;
        healthbar.InitializeHealthBar(healthComponent, "Player");
    }
    #endregion

    #region Enable/Disable
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }
    #endregion

    #region Updates
    private void Update()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case PlayerState.Normal:
                HandleAiming();
                UpdateAimAnimation();
                HandleMovement();
                HandleGravity();
                HandleJump();
                HandleDash();
                HandleScrollThroughWeapons();
                HandleAttack();

                previousAiming = aiming;
                break;

            case PlayerState.Dashing:
                HandleDashMovement();
                HandleScrollThroughWeapons();
                break;

            case PlayerState.Attacking:
                HandleGravity();
                HandleAttackMovement();
                break;

            case PlayerState.Dead:
                //slowly move camera out
                deathCamera.GetComponent<CinemachineThirdPersonFollow>().CameraDistance 
                    = Mathf.MoveTowards(deathCamera.GetComponent<CinemachineThirdPersonFollow>().CameraDistance, 10f, 0.5f * Time.deltaTime);
                break;
        }
    }

    private void LateUpdate()
    {
        CameraRotation();

        if (brain == null || followCamera == null)
        {
            isFreelookEnabled = false;
            return;
        }

        // If Cinemachine is currently blending,
        // the blend to our target camera has NOT finished yet.
        if (brain.IsBlending)
        {
            isFreelookEnabled = false;
            return;
        }

        // There is no active blend.
        // Check whether our target camera is now the active camera.
        var activeCamera = brain.ActiveVirtualCamera;

        isFreelookEnabled =
            activeCamera != null &&
            activeCamera.Name == followCamera.Name;
    }
    #endregion


    #region Dashing
    private void HandleDash()
    {
        if (!inputActions.Player.Dash.WasPressedThisFrame())
            return;

        if (dashCooldownTimer > 0f)
            return;

        StartDash();
    }

    private void StartDash()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 direction;

        if (input.sqrMagnitude > threshold)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            direction =
                cameraForward * input.y +
                cameraRight * input.x;
        }
        else
        {
            direction = transform.forward;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude <= threshold)
            direction = transform.forward;

        dashDirection = direction.normalized;

        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        currentState = PlayerState.Dashing;
    }

    private void HandleDashMovement()
    {
        controller.Move(dashDirection * dashSpeed * Time.deltaTime);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            currentState = PlayerState.Normal;
        }
    }
    #endregion


    #region HandeMovement
    private void HandleMovement()
    {
        
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();

        //Animate walk cycle
        if (controller.isGrounded)
            animator.SetBool("IsWalking", input.sqrMagnitude > threshold);
        else
            animator.SetBool("IsWalking", false);

        if (isFreelookEnabled) //!aiming
        {
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

            if (inputDirection.sqrMagnitude < threshold)
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
            if (moveDirection.sqrMagnitude > threshold)
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
        else
        {
            HandleLook();
            Vector3 move = transform.right * input.x + transform.forward * input.y;
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (inputActions.Player.Jump.WasPressedThisFrame())
        {
            if (controller.isGrounded)
            {
                canDoubleJump = true;
                Jump();
                return;
            }

            if (!controller.isGrounded && canDoubleJump)
            {
                canDoubleJump = false;
                Jump();
            }
        }
    }

    private void Jump()
    {
        animator.Play("Jump", 0, 0f);
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
    #endregion


    #region Attacking
    private void HandleAttack()
    {
        if (weaponController.currentWeapon == null)
            return;

        if (!inputActions.Player.Attack.WasPressedThisFrame())
            return;

        if (weaponController.currentWeapon.weaponType == Weapon.WeaponType.Melee)
        {
            currentState = PlayerState.Attacking;
            weaponController.currentWeapon.PlayAttackAnimation();
            StartAttack();
            return;
        }

        if (weaponController.currentWeapon.weaponType == Weapon.WeaponType.Ranged)
        {
            if (aiming)
            {
                weaponController.currentWeapon.PlayAttackAnimation();
                return;
            }
        }
    }

    private void StartAttack()
    {
        currentState = PlayerState.Attacking;

        attackTimer = attackDuration;

        // Get the camera's forward direction.
        Vector3 cameraForward = cameraTransform.forward;

        // Ignore vertical camera rotation.
        cameraForward.y = 0f;

        if (cameraForward.sqrMagnitude > threshold)
        {
            cameraForward.Normalize();

            // Immediately face the camera's direction.
            transform.rotation = Quaternion.LookRotation(cameraForward);

            // Lock the attack movement direction.
            attackDirection = cameraForward;
        }
        else
        {
            attackDirection = transform.forward;
        }

        attackStartPosition = transform.position;

        // Stop normal movement immediately.
        currentSpeed = 0f;
    }

    private void HandleAttackMovement()
    {
        float progress = 1f - (attackTimer / attackDuration);
        progress = Mathf.Clamp01(progress);

        // Move quickly at the beginning, then slow down.
        float movementMultiplier = 1f - progress;

        float moveAmount =
            attackForwardDistance *
            movementMultiplier *
            Time.deltaTime /
            attackDuration;

        controller.Move(attackDirection * moveAmount);

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            currentState = PlayerState.Normal;
        }
    }

    public void EndAttack()
    {
        currentState = PlayerState.Normal;
        weaponController.currentWeapon.OnAnimationFinished();
    }

    #endregion


    #region CameraFunctionality
    private void HandleLook()
    {
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        // Detect whether input came from a mouse
        if (inputActions.Player.Look.activeControl?.device is Mouse)
        {
            lookInput *= mouseSensitivity;
        }
        else
        {
            lookInput *= controllerSensitivity * Time.deltaTime;
        }

        // Rotate player left/right
        transform.Rotate(Vector3.up * lookInput.x);
    }

    private void HandleAiming()
    {
        aiming = inputActions.Player.Aim.IsPressed();

        if (weaponController.currentWeapon.weaponType != Weapon.WeaponType.Ranged)
            return;

        if (aiming && !previousAiming)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;

            if (cameraForward.sqrMagnitude > threshold)
            {
                transform.rotation = Quaternion.LookRotation(cameraForward);
            }
        }
        SetAiming(aiming);
    }

    public void SetAiming(bool aiming)
    {
        //Transition between camera views
        aimCamera.Priority = aiming ? 20 : 10;
        followCamera.Priority = aiming ? 10 : 20;
    }

    private void UpdateAimAnimation()
    {
        if (animator == null)
            return;

        if (weaponController.currentWeapon.weaponType != Weapon.WeaponType.Ranged)
            return;

        float targetLayerWeight = aiming ? 1f : 0f;

        currentAimLayerWeight = Mathf.MoveTowards(
            currentAimLayerWeight,
            targetLayerWeight,
            aimLayerBlendSpeed * Time.deltaTime
        );

        animator.SetLayerWeight(
            aimLayerIndex,
            currentAimLayerWeight
        );

        // cinemachineTargetPitch is already controlled by the mouse
        float aimPitch = Mathf.Clamp(
            cinemachineTargetPitch,
            aimDownLimit,
            aimUpLimit
        );

        animator.SetFloat("AimPitch", aimPitch);
    }


    private void CameraRotation()
    {
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();
        // if there is an input and camera position is not fixed
        if (lookInput.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            if (inputActions.Player.Look.activeControl?.device is Mouse)
            {
                lookInput *= mouseSensitivity;
            }
            else
            {
                lookInput *= controllerSensitivity * Time.deltaTime;
            }

            cinemachineTargetYaw += lookInput.x;
            cinemachineTargetPitch -= lookInput.y;
        }

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        followCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride,
            cinemachineTargetYaw, 0.0f);

        aimCameraTarget.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);
    }

    #endregion


    private void HandleScrollThroughWeapons()
    {
        Vector2 scrollInput = inputActions.UI.ScrollWheel.ReadValue<Vector2>();

        if (scrollInput.y > 0f)
        {
            int nextIndex = (weaponController.currentWeaponIndex + 1) % weaponController.slots.Count;
            weaponController.EquipWeapon(nextIndex);
        }
        else if (scrollInput.y < 0f)
        {
            int previousIndex = (weaponController.currentWeaponIndex - 1 + weaponController.slots.Count) % weaponController.slots.Count;
            weaponController.EquipWeapon(previousIndex);
        }
    }

    public void Die()
    {
        if (currentState == PlayerState.Dead)
            return;

        //animator.SetTrigger("Die");
        deathCamera.Priority = 30;
        currentState = PlayerState.Dead;
        controller.enabled = false;
        inputActions.Player.Disable();
        animator.StopPlayback();
        animator.enabled = false;

        HUDManager.Instance.OnPlayerDeath();
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}