using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Archer : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float arrowVelocity = 20f;
    [SerializeField] private float attackRange = 100f;
    [SerializeField] private float targetTooCloseDistance = 15f;
    [SerializeField] private float retreatDistance = 30f;
    [SerializeField] private float retreatTimeout = 3.5f;

    [SerializeField] private float strafeDistance = 3f;
    [SerializeField] private float startStrafeDistance = 25f;
    [SerializeField] private float strafeDistanceTolerance = 0.5f;
    [SerializeField] private float strafeCooldown = 0.5f;

    [SerializeField] private float aimRotationSpeed = 5f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float retreatRotateSpeed = 360f;

    private NavMeshAgent agent;
    private Transform target;
    private float shootCooldownTimer;
    private float strafeCooldownTimer;


    private enum ArcherState
    {
        Idle,
        Repositioning,
        Shooting,
        Stagger,
        Dead
    }
    private ArcherState state = ArcherState.Shooting; 

    //stuff for calculating ballistic trajectory
    [SerializeField] private float arcHeight = 3f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootCooldownTimer = attackCooldown;
        strafeCooldownTimer = strafeCooldown;
        agent.speed = moveSpeed;

        if (PlayerController.Instance != null)
            target = PlayerController.Instance.transform;
    }

    private void Update()
    {
        switch (state)
        {
            case ArcherState.Idle:
                
                break;

            case ArcherState.Repositioning:
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    StopCoroutine(RetreatTimeout());
                    agent.ResetPath();
                    state = ArcherState.Shooting;
                }
                break;

            case ArcherState.Shooting:
                
                HandleStrafe();

                HandleShooting();

                float distanceToPlayer = CheckDistanceToPlayer();
                if (distanceToPlayer < targetTooCloseDistance)
                {
                    Retreat();
                }
                break;

            case ArcherState.Stagger:
                
                break;

            case ArcherState.Dead:
                
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
            aimRotationSpeed * Time.deltaTime
        );
    }

    #region Shooting Logic
    private void ShootArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            if (!CalculateArrowVelocity(out Vector3 launchVelocity))
            {
                Debug.LogWarning("Cannot calculate a valid launch velocity for the arrow.");
                return;
            }
                

            GameObject arrow = Instantiate(
                arrowPrefab,
                arrowSpawnPoint.position,
                Quaternion.LookRotation(launchVelocity)
            );

            Rigidbody rb = arrow.GetComponent<Rigidbody>();

            if (rb == null)
            {
                Destroy(arrow);
                Debug.LogError("Arrow prefab is missing a Rigidbody component.");
                return;
            }

            rb.linearDamping = 0f; //arrowLinearDamping;
            rb.linearVelocity = launchVelocity;

            Projectile projectile = arrow.GetComponent<Projectile>();

            if (projectile != null)
                projectile.Initialize(10, gameObject);

            Destroy(arrow, 5f);
        }
    }

    private bool CalculateArrowVelocity(out Vector3 velocity)
    {
        velocity = Vector3.zero;

        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 targetPosition =
            target.position + Vector3.up;

        Vector3 horizontalDisplacement =
            new Vector3(
                targetPosition.x - arrowSpawnPoint.position.x,
                0f,
                targetPosition.z - arrowSpawnPoint.position.z
            );

        float launchHeight = arrowSpawnPoint.position.y;
        float targetHeight = targetPosition.y;

        // Place the apex above whichever point is higher.
        float apexHeight =
            Mathf.Max(launchHeight, targetHeight) + arcHeight;

        float riseToApex = apexHeight - launchHeight;
        float fallToTarget = apexHeight - targetHeight;

        if (riseToApex < 0f || fallToTarget < 0f)
        {
            Debug.LogWarning("Invalid rise or fall height for ballistic trajectory.");
            return false;
        }

        float timeToApex =
            Mathf.Sqrt(2f * riseToApex / gravity);

        float timeFromApex =
            Mathf.Sqrt(2f * fallToTarget / gravity);

        float totalFlightTime =
            timeToApex + timeFromApex;

        if (totalFlightTime <= 0f ||
            float.IsNaN(totalFlightTime))
        {
            Debug.LogWarning("Total flight time is invalid for ballistic trajectory.");
            return false;
        }

        Vector3 horizontalVelocity =
            horizontalDisplacement / totalFlightTime;

        Vector3 verticalVelocity =
            Vector3.up * Mathf.Sqrt(2f * gravity * riseToApex);

        velocity = horizontalVelocity + verticalVelocity;

        return true;
    }

    private void HandleShooting()
    {
        if (target == null)
            return;

        FacePlayer();

        shootCooldownTimer -= Time.deltaTime; //cooldown is temporary until we get the archer animations in

        float distanceToTarget = CheckDistanceToPlayer();
        if (shootCooldownTimer <= 0f && distanceToTarget <= attackRange)
        {
            ShootArrow(); //make this an animation event later
            shootCooldownTimer = attackCooldown;
        }
    }
    
    private float CheckDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, target.position);
    }
    #endregion


    #region Movement Logic
    private void HandleStrafe()
    {
        if (target == null)
            return;

        agent.angularSpeed = 0f;
        float distanceToPlayer = CheckDistanceToPlayer();
        strafeCooldownTimer -= Time.deltaTime;

        if (distanceToPlayer > startStrafeDistance)
            return;

        float heightThreshold = 5f;
        if (Mathf.Abs(transform.position.y - target.position.y) > heightThreshold)
            return;

        if (strafeCooldownTimer <= 0f)
        {
            Strafe();
            strafeCooldownTimer = strafeCooldown;
        }
    }

    private void Strafe()
    {
        if (target == null)
            return;
        
        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        Vector3 strafeDirection = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        
        // Randomly choose left or right strafe
        if (Random.value > 0.5f)
            strafeDirection = -strafeDirection;
        
        Vector3 newPosition = transform.position + strafeDirection * strafeDistance;
        agent.SetDestination(newPosition);
        if (agent.remainingDistance > strafeDistance + strafeDistanceTolerance)
            agent.ResetPath(); // If the new position is unreachable, stop moving
    }

    private void Retreat()
    {
        if (target == null)
            return;

        agent.angularSpeed = retreatRotateSpeed;

        StartCoroutine(RetreatTimeout()); //in case the path is too far
        state = ArcherState.Repositioning;

        Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
        Vector3 newPosition = transform.position + directionAwayFromPlayer * retreatDistance;
        agent.SetDestination(newPosition);
    }

    private IEnumerator RetreatTimeout()
    {
        yield return new WaitForSeconds(retreatTimeout);
        agent.ResetPath();
        state = ArcherState.Shooting;
    }
    #endregion

    public void OnDeath()
    {
        Destroy(gameObject);
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetTooCloseDistance);
    }
}
