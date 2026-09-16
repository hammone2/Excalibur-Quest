using UnityEngine;
using UnityEngine.AI;

public class Orc : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float chargeSpeed = 20f;
    [SerializeField] private float acceleration = 5f;

    [Header("References")]
    [SerializeField] private Animator animator;

    private float currentSpeed = 0f;
    private NavMeshAgent agent;
    private CharacterController characterController;
    private Transform target;
    private bool touchingWall;

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
    }

    private void Start()
    {
        target = PlayerController.Instance.transform;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case OrcState.Idle:
                if (distance <= detectionRange)
                {
                    StartAiming();
                }
                break;

            case OrcState.Aim:
                FacePlayer();
                break;

            case OrcState.Attack:
                Vector3 direction = transform.forward;
                
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    chargeSpeed,
                    acceleration * Time.deltaTime
                );

                characterController.Move(direction * chargeSpeed * Time.deltaTime);

                if (touchingWall)
                {
                    StartRecover();
                }
                break;

            case OrcState.Recover:
                // Recovery logic here
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
        currentState = OrcState.Aim;

        if (target == null) return;

        Invoke(nameof(StartAttack), 3f);
    }

    private void StartAttack()
    {
        if (currentState != OrcState.Dead)
        {
            //animator.SetTrigger("Attack");
            currentState = OrcState.Aim;
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
            characterController.enabled = true;
            currentState = OrcState.Attack;
        }
    }

    private void StartRecover()
    {
        currentSpeed = 0f;
        touchingWall = false;
        characterController.enabled = false;
        agent.enabled = true;
        currentState = OrcState.Recover;

        Invoke(nameof(StartIdle), 1f);
    }

    private void StartIdle()
    {
        currentState = OrcState.Idle;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (currentState != OrcState.Attack) return;

        // Ignore floors and ceilings
        if (hit.normal.y < 0.7f)
        {
            touchingWall = true;
        }
    }
}
