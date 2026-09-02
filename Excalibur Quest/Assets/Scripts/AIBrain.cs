using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AIBrain : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float detectionDistance = 20f;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private Transform target;

    private enum EnemyState
    {
        Idle,
        Patrol,
        Search,
        Chase,
        Attack,
        Stun,
        Dead
    }
    private EnemyState currentState = EnemyState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandlePlayerDetection();
                break;

            case EnemyState.Patrol:
                HandlePlayerDetection();
                break;

            case EnemyState.Search:
                HandlePlayerDetection();
                break;

            case EnemyState.Chase:
                target = PlayerController.Instance.transform;
                agent.SetDestination(target.position);

                if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) <= attackDistance)
                {
                    StartCoroutine(StartAttack());
                }
                    
                break;

            case EnemyState.Attack:
                
                Quaternion targetRotation = Quaternion.LookRotation(PlayerController.Instance.transform.position - transform.position);

                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                
                );



                break;

            case EnemyState.Stun:
                break;

            case EnemyState.Dead:
                break;
        }
    }

    private void HandlePlayerDetection()
    {
        if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) <= detectionDistance)
            currentState = EnemyState.Chase;
    }

    private IEnumerator StartAttack()
    {
        agent.isStopped = true;
        agent.ResetPath();
        currentState = EnemyState.Attack;
        Debug.Log("Enemy is attacking!");
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;
        currentState = EnemyState.Chase;
    }
}
