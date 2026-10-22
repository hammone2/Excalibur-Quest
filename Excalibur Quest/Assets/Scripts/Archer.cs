using UnityEngine;
using UnityEngine.AI;

public class Archer : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float arrowVelocity = 20f;

    [SerializeField] private float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private Transform target;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (PlayerController.Instance != null)
            target = PlayerController.Instance.transform;
    }


    void Update()
    {
        FacePlayer();

        attackCooldown -= Time.deltaTime;
        if (target != null)
        {
            if (attackCooldown <= 0f)
            {
                ShootArrow();
                attackCooldown = 1.5f; // Reset cooldown
            }
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
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = arrowSpawnPoint.forward * arrowVelocity; // Adjust speed as needed
                Destroy(arrow, 5f); // Adjust time as needed
            }
        }
    }
}
