using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AIBrain : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Repositioning")]
    [SerializeField] private float circleRadius = 19f;


    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;

    private NavMeshAgent agent;
    private Transform target;

    private float attackTimer;
    private bool isRepositioning = false;

    private float distance;

    private enum EnemyState
    {
        Idle,
        Patrol,
        Search,
        Chase,
        Reposition,
        Attack,
        Recover,
        Stagger,
        Dead
    }
    private EnemyState currentState = EnemyState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
    }

    private void Start()
    {
        if (PlayerController.Instance != null)
            target = PlayerController.Instance.transform;
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (target)
        {
            distance = Vector3.Distance(
                transform.position,
                target.position
            );
        }


        switch (currentState)
        {
            case EnemyState.Idle:
                if (target)
                    HandlePlayerDetection(distance);
                break;

            case EnemyState.Patrol:
                HandlePlayerDetection(distance);
                break;

            case EnemyState.Search:
                HandlePlayerDetection(distance);
                break;

            case EnemyState.Chase:
                UpdateChase(distance);
                    
                break;

            case EnemyState.Attack:
                FacePlayer();

                break;

            case EnemyState.Reposition:
                UpdateReposition(distance);
                break;

            case EnemyState.Recover:
                UpdateRecover();
                break;

            case EnemyState.Stagger:
                break;

            case EnemyState.Dead:
                break;
        }
    }

    private void HandlePlayerDetection(float distance)
    {
        if (distance <= detectionRange)
            ChangeState(EnemyState.Chase);
    }


    private void UpdateChase(float distance)
    {
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);

            SetMovingAnimation(true);
            return;
        }

        SetMovingAnimation(false);

        if (attackTimer <= 0)
        {
            StartAttack();
        }
    }

    #region Reposition logic
    private void UpdateReposition(float distance)
    {
        if (distance > detectionRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (distance <= attackRange && attackTimer <= 0)
        {
            StartAttack();
        }

        if (isRepositioning) return;


        StartCoroutine(Reposition());
        
    }

    private IEnumerator Reposition()
    {
        isRepositioning = true;

        float distance = Random.Range(10f, circleRadius);
        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 randomPoint = transform.position + new Vector3(direction.x * distance, 0f, direction.y * distance);

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomPoint,
            out hit,
            12f,
            NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }

        SetMovingAnimation(true);

        yield return new WaitForSeconds(Random.Range(0.5f, 5f));

        isRepositioning = false;
    }
    #endregion

    private void UpdateRecover()
    {
        FacePlayer();

        if (attackTimer <= 0)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    // Called by an Animation Event
    public void AttackFinished()
    {
        weapon.OnAnimationFinished();
        attackTimer = attackCooldown;
        agent.isStopped = false;
        ChangeState(EnemyState.Recover);
    }

    public void TakeHit()
    {
        if (currentState == EnemyState.Dead)
            return;

        agent.isStopped = true;

        ChangeState(EnemyState.Stagger);

        animator.SetTrigger("Hit");
    }

    // Called by an Animation Event
    public void StaggerFinished()
    {
        if (currentState == EnemyState.Dead)
            return;

        ChangeState(EnemyState.Chase);
    }

    public void Die()
    {
        if (currentState == EnemyState.Dead)
            return;

        currentState = EnemyState.Dead;
        weapon.OnAnimationFinished();

        agent.isStopped = true;

        GetComponent<Collider>().enabled = false;

        animator.StopPlayback();
        animator.enabled = false;
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

    private void SetMovingAnimation(bool moving)
    {
        if (animator != null)
            animator.SetBool("IsWalking", moving);
    }


    private void StartAttack()
    {
        if (currentState != EnemyState.Dead) //doing this so the enemy wont become a zombie after dying
        {
            //animator.SetTrigger("Attack");
            weapon.PlayAttackAnimation();
            agent.isStopped = true;
            agent.ResetPath();
            currentState = EnemyState.Attack;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }
}
