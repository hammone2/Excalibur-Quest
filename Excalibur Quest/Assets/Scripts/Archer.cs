using UnityEngine;
using UnityEngine.AI;

public class Archer : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float arrowVelocity = 20f;
    [SerializeField] private float arrowLinearDamping = 2f;
    [SerializeField] private float attackRange = 100f;

    [SerializeField] private float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private Transform target;
    private float cooldownTimer;


    private enum ArcherState
    {
        Idle,
        Repositioning,
        Shooting,
        Stagger,
        Dead
    }
    private ArcherState state = ArcherState.Idle; 

    //stuff for calculating ballistic trajectory
    [SerializeField] private float arcHeight = 3f;
    //public float yMax;
    //private float g = 9.8f; // gravity
    //private float y_0;
    //private float x;
    //private float z;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cooldownTimer = attackCooldown;


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
                
                break;

            case ArcherState.Shooting:
                
                break;

            case ArcherState.Stagger:
                
                break;

            case ArcherState.Dead:
                
                break;
        }



        if (target == null)
            return;

        FacePlayer();

        cooldownTimer -= Time.deltaTime; //cooldown is temporary until we get the archer animations in

        Vector3 distanceToTarget = target.position - transform.position;
        if (cooldownTimer <= 0f && distanceToTarget.magnitude <= attackRange)
        {
            ShootArrow(); //make this an animation event later
            cooldownTimer = attackCooldown;
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

    private void ShootArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            /*GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                y_0 = arrow.transform.position.y - (target.position.y + 1); //1 is the offset for the target's height, assuming the target is a humanoid character
                x = target.position.x - arrowSpawnPoint.position.x;
                z = target.position.z - arrowSpawnPoint.position.z;

                //rb.linearDamping = arrowLinearDamping;
                rb.linearDamping = 0f;

                arrow.GetComponent<Projectile>().Initialize(10, gameObject);
                rb.linearVelocity = CalculateVelocity(); //arrowSpawnPoint.forward * arrowVelocity;
                Destroy(arrow, 5f);
            }*/

            if (!TryCalculateVelocity(out Vector3 launchVelocity))
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

    /*private Vector3 CalculateVelocity()
    {
        // Distance X and Z to target location
        Vector3 displacementXZ = new Vector3(x, 0, z);

        // Implement equations derived from kinematic analysis
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * g * (yMax - y_0));
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(2 * (yMax - y_0) / g) + Mathf.Sqrt(2 * yMax / g));

        Vector3 velocity = velocityXZ + velocityY;
        return velocity;
    }*/

    private bool TryCalculateVelocity(out Vector3 velocity)
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

    public void OnDeath()
    {
        Destroy(gameObject);
    }
}
