using UnityEngine;
using UnityEngine.AI;

public class Wizard : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private float teleportInterval = 5f;
    [SerializeField] private float minimumDistanceFromPlayer = 5f;
    [SerializeField] private float maximumDistanceFromPlayer = 12f;
    [SerializeField] private int positionAttempts = 30;
    [SerializeField] private float navMeshSampleDistance = 3f;

    [Header("Vertical Teleport Range")]
    [SerializeField] private float minimumVerticalOffset = 2f;
    [SerializeField] private float maximumVerticalOffset = 8f;

    [Header("Optional")]
    [SerializeField] private bool facePlayerAfterTeleport = true;

    [Header("Summons")]
    [SerializeField] private GameObject[] summonPrefab;
    [Min(1)]
    [SerializeField] private int maxActiveSummons = 7;
    private int currentActiveSummons = 0;
    [SerializeField] private int summonMaxAmount = 3;
    [Min(1)]
    [SerializeField] private int summonMinAmount = 1;
    [SerializeField] private float summonMaxRange = 10;
    [SerializeField] private float summonMinRange = 5;

    [Header("Fireball")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballCooldown = 3f;
    [SerializeField] private float fireballSpeed = 10f;
    [SerializeField] private int fireballDamage = 10;
    [SerializeField] private float fireballTimeAlive = 15f;

    private NavMeshAgent agent;
    private Transform target;
    private float teleportTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        teleportTimer = teleportInterval;

        if (PlayerController.Instance != null)
            target = PlayerController.Instance.transform;
    }

    private void Update()
    {
        if (target == null)
            return;

        teleportTimer -= Time.deltaTime;

        if (teleportTimer <= 0f)
        {
            TeleportAroundPlayer();
            teleportTimer = teleportInterval;
        }
    }

    private void TeleportAroundPlayer()
    {
        for (int i = 0; i < positionAttempts; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(
                minimumDistanceFromPlayer,
                maximumDistanceFromPlayer
            );

            float verticalOffset = Random.Range(
                minimumVerticalOffset,
                maximumVerticalOffset
            );

            // Randomly teleport above or below the player
            verticalOffset *= Random.value < 0.5f ? -1f : 1f;

            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            Vector3 desiredPosition = target.position + direction * distance;
            desiredPosition.y += verticalOffset;

            if (NavMesh.SamplePosition(
                    desiredPosition,
                    out NavMeshHit hit,
                    navMeshSampleDistance,
                    NavMesh.AllAreas))
            {
                float horizontalDistance = Vector3.Distance(
                    new Vector3(hit.position.x, 0f, hit.position.z),
                    new Vector3(target.position.x, 0f, target.position.z)
                );

                float verticalDistance = Mathf.Abs(
                    hit.position.y - target.position.y
                );

                if (horizontalDistance < minimumDistanceFromPlayer ||
                    horizontalDistance > maximumDistanceFromPlayer ||
                    verticalDistance < minimumVerticalOffset)
                {
                    continue;
                }

                if (agent != null && agent.isOnNavMesh)
                {
                    agent.Warp(hit.position);
                }
                else
                {
                    transform.position = hit.position;
                }

                if (facePlayerAfterTeleport)
                {
                    FacePlayer();
                    SummonEnemy();
                }

                return;
            }
        }

        Debug.LogWarning(
            $"{name} could not find a valid NavMesh teleport position."
        );
    }

    private void SummonEnemy()
    {
        if (summonPrefab.Length == 0 || target == null)
            return;

        if (currentActiveSummons >= maxActiveSummons)
        {
            FireballAttack();
            return;
        }

        int amountToSummon = Random.Range(summonMinAmount, summonMaxAmount + 1);
        amountToSummon = Mathf.Min(amountToSummon, maxActiveSummons - currentActiveSummons);

        for (int i = 0; i < amountToSummon; i++)
        {
            Vector3 summonPosition = Vector3.zero;
            bool foundPosition = false;

            for (int j = 0; j < positionAttempts; j++)
            {
                float angle = Random.Range(0f, 360f);
                float distance = Random.Range(summonMinRange, summonMaxRange);

                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                Vector3 desiredPosition = transform.position + direction * distance;

                if (NavMesh.SamplePosition(
                        desiredPosition,
                        out NavMeshHit hit,
                        navMeshSampleDistance,
                        NavMesh.AllAreas))
                {
                    float horizontalDistance = Vector3.Distance(
                        new Vector3(hit.position.x, 0f, hit.position.z),
                        new Vector3(transform.position.x, 0f, transform.position.z)
                    );

                    //if (horizontalDistance < summonMinAmount || horizontalDistance > summonMaxAmount)
                    //continue;

                    summonPosition = hit.position;
                    foundPosition = true;
                    break;
                }
            }

            if (foundPosition)
            {
                int randomIndex = Random.Range(0, summonPrefab.Length);
                GameObject enemy = Instantiate(
                    summonPrefab[randomIndex],
                    summonPosition,
                    Quaternion.identity
                );

                currentActiveSummons++;
                enemy.AddComponent<SummonedEntity>().Initialize(this);
            }
            else
            {
                Debug.LogWarning($"{name} could not find a valid NavMesh summon position.");
            }
        }
    }

    private void FireballAttack()
    {
        if (fireballPrefab == null || target == null)
            return;

        GameObject fireball = Instantiate(
            fireballPrefab,
            transform.position + Vector3.up * 1.5f,
            Quaternion.identity
        );

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.Initialize(
                target,
                fireballSpeed,
                fireballDamage,
                fireballTimeAlive,
                gameObject,
                0f
            );
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void OnSummonedEnemyDestroyed()
    {
        currentActiveSummons--;
    }
}
