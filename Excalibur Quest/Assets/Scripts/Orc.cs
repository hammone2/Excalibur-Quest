using UnityEngine;
using UnityEngine.AI;

public class Orc : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float chaseCooldown = 1.5f;
    [SerializeField] private float chaseTime = 3f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float chargeSpeed = 20f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private LayerMask wallLayerMask;
    

    [Header("Ground")]
    [SerializeField] private float groundedForce = -2f;
    [SerializeField] private float gravity = -25f;

    [Header("Box Check Settings")]
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 1f, 0.5f);
    [SerializeField] private Vector3 boxOffset = new Vector3(0f, 1.7f, 0.75f);

    [Header("References")]
    [SerializeField] private Animator animator;

    private float currentSpeed = 0f;
    private NavMeshAgent agent;
    private CharacterController characterController;
    private Transform target;
    private bool touchingWall;
    private float currentAttackTime;
    private Vector3 velocity;
    private float currentAimTime = 0f;
    private float aimTime = 3f;

    private OrcRigAnimEventController animEventController;

    enum OrcState
    {
        Idle,
        Patrol,
        Search,
        Chase,
        Reposition,
        Aim,
        Attack,
        Recover,
        Stagger,
        Dead
    }
    private OrcState currentState = OrcState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;

        characterController = GetComponent<CharacterController>();

        animEventController = GetComponentInChildren<OrcRigAnimEventController>();
    }

    private void Start()
    {
        target = PlayerController.Instance.transform;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        Debug.Log(currentState);
        switch (currentState)
        {
            case OrcState.Idle:

                if (distance <= detectionRange && currentState != OrcState.Attack)
                {
                    StartAiming();
                }

                break;

            case OrcState.Aim:
                FacePlayer();

                if (distance <= attackRange)
                {
                    StartAttack();
                    return;
                }

                currentAimTime += Time.deltaTime;
                Debug.Log(currentAimTime);
                if (currentAimTime >= aimTime)
                {
                    StartChase();
                }


                
                break;

            case OrcState.Chase:
                Vector3 direction = transform.forward;
                
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    chargeSpeed,
                    acceleration * Time.deltaTime
                );

                characterController.Move(direction * chargeSpeed * Time.deltaTime);
                HandleGravity();

                // Box check from eye level forward to detect wall or player
                Vector3 boxCenter = transform.position + transform.rotation * boxOffset;
                touchingWall = Physics.CheckBox(
                    boxCenter,
                    boxHalfExtents,
                    transform.rotation,
                    wallLayerMask
                );

                // Countdown attack time
                currentAttackTime -= Time.deltaTime;
                if (touchingWall || currentAttackTime <= 0f)
                {
                    StartRecover();
                }

                break;

            case OrcState.Recover:
                // Recovery logic here
                break;

            case OrcState.Attack:
                FacePlayer();
                break;

            case OrcState.Dead:
                // Dead logic here
                break;
        }
    }

    private void FacePlayer()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;

        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void StartAiming()
    {
        if (currentState == OrcState.Attack)
            return;

        currentState = OrcState.Aim;
        animator.SetTrigger("LookLeft");

        currentAimTime = 0f;
    }

    private void StartChase()
    {
        if (currentState != OrcState.Dead)
        {
            animator.SetTrigger("Run");
            currentState = OrcState.Aim;
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
            characterController.enabled = true;
            currentAttackTime = chaseTime;
            currentState = OrcState.Chase;
        }
    }

    private void StartRecover()
    {
        currentSpeed = 0f;
        touchingWall = false;
        characterController.enabled = false;
        agent.enabled = true;
        currentState = OrcState.Recover;

        //Invoke(nameof(StartIdle), 1f);
        animator.SetTrigger("WornOut");
    }

    public void StartAttack()
    {
        animator.ResetTrigger("LookLeft");
        currentState = OrcState.Attack;
        animator.SetTrigger("Attack");
    }

    public void StartIdle()
    {
        currentState = OrcState.Idle;
        animator.SetTrigger("Idle");
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded && velocity.y < 0f)
        {
            velocity.y = groundedForce;
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(
            velocity * Time.deltaTime
        );
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the box used for wall detection
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxOffset, boxHalfExtents * 2f);
        }
        else
        {
            // Preview in editor using current transform
            Matrix4x4 matrix = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                Vector3.one
            );
            Gizmos.matrix = matrix;
            Gizmos.DrawWireCube(boxOffset, boxHalfExtents * 2f);
        }
    }
}
