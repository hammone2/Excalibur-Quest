using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private int damage = 10;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask damageableLayers = ~0;
    [SerializeField] private LayerMask lineOfSightLayers = ~0;

    [Header("Lifetime")]
    [SerializeField] private float destroyAfterSeconds = 2f;

    private GameObject attacker;
    private bool canDamageAttacker = true;
    private bool hasExploded = false;

    public void Initialize(
        int explosionDamage,
        GameObject explosionAttacker,
        float explosionRadius,
        LayerMask explosionDamageableLayers,
        LayerMask explosionLineOfSightLayers,
        bool allowAttackerDamage
    )
    {
        damage = explosionDamage;
        attacker = explosionAttacker;
        radius = explosionRadius;
        damageableLayers = explosionDamageableLayers;
        lineOfSightLayers = explosionLineOfSightLayers;
        canDamageAttacker = allowAttackerDamage;

        Explode();
    }

    private void Start()
    {
        // Allows the prefab to work by itself if Initialize was not called.
        if (!hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;

        Collider[] collidersInRadius = Physics.OverlapSphere(
            transform.position,
            radius,
            damageableLayers,
            QueryTriggerInteraction.Collide
        );

        HashSet<IDamageable> damagedEntities = new HashSet<IDamageable>();

        foreach (Collider targetCollider in collidersInRadius)
        {
            if (!TryGetDamageable(
                    targetCollider,
                    out IDamageable damageable,
                    out GameObject damageableObject))
            {
                continue;
            }

            // Prevent multiple colliders on the same entity from causing
            // multiple damage calls.
            if (damagedEntities.Contains(damageable))
            {
                continue;
            }

            // Prevent the explosion from damaging its attacker if requested.
            if (!canDamageAttacker &&
                attacker != null &&
                IsSameEntity(attacker, damageableObject))
            {
                continue;
            }

            Vector3 targetPoint = targetCollider.ClosestPoint(transform.position);
            Vector3 directionToTarget = targetPoint - transform.position;

            if (directionToTarget.sqrMagnitude <= 0.001f)
            {
                directionToTarget = damageableObject.transform.position - transform.position;
            }

            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= 0.001f)
            {
                directionToTarget = transform.forward;
                distanceToTarget = 0f;
            }
            else
            {
                directionToTarget.Normalize();
            }

            // Check whether something blocks the explosion.
            if (!HasLineOfSight(
                targetCollider,
                damageableObject,
                directionToTarget,
                distanceToTarget))
            {
                continue;
            }

            DamageInfo damageInfo = new DamageInfo(
                damage,
                targetPoint,
                directionToTarget,
                attacker
            );

            Debug.Log($"Explosion at {transform.position} dealing {damage} damage to {damageableObject.name} at {targetPoint}");

            damageable.TakeDamage(damageInfo);
            damagedEntities.Add(damageable);
        }

        Destroy(gameObject, destroyAfterSeconds);
    }

    private bool HasLineOfSight(
        Collider targetCollider,
        GameObject damageableObject,
        Vector3 direction,
        float distance
    )
    {
        if (distance <= 0.001f)
        {
            return true;
        }

        if (!Physics.Raycast(
                transform.position,
                direction,
                out RaycastHit hit,
                distance + 0.05f,
                lineOfSightLayers,
                QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        // The first thing hit must belong to the intended damageable entity.
        return hit.collider == targetCollider ||
               hit.collider.transform.IsChildOf(damageableObject.transform) ||
               damageableObject.transform.IsChildOf(hit.collider.transform);
    }

    private bool TryGetDamageable(
        Collider targetCollider,
        out IDamageable damageable,
        out GameObject damageableObject
    )
    {
        MonoBehaviour[] components =
            targetCollider.GetComponentsInParent<MonoBehaviour>(true);

        foreach (MonoBehaviour component in components)
        {
            if (component is IDamageable foundDamageable)
            {
                damageable = foundDamageable;
                damageableObject = component.gameObject;
                return true;
            }
        }

        damageable = null;
        damageableObject = null;
        return false;
    }

    private bool IsSameEntity(GameObject first, GameObject second)
    {
        return first == second ||
               first.transform.IsChildOf(second.transform) ||
               second.transform.IsChildOf(first.transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}