using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 25;
    [SerializeField] private BoxCollider hitbox;
    [SerializeField] private LayerMask damageableLayers;

    [HideInInspector] public bool active;
    private GameObject attacker;

    private readonly HashSet<IDamageable> hitTargets = new();

    private void Awake()
    {
        active = false;
    }

    public void BeginAttack(GameObject attacker)
    {
        this.attacker = attacker;
        active = true;
        hitTargets.Clear();
        CheckForTargets();
    }

    public void EndAttack()
    {
        active = false;
        attacker = null;
        hitTargets.Clear();
    }

    private void CheckForTargets()
    {
        Collider[] colliders = Physics.OverlapBox(
            hitbox.bounds.center,
            hitbox.bounds.extents,
            hitbox.transform.rotation,
            damageableLayers
        );

        foreach (Collider collider in colliders)
        {
            TryDamage(collider);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (!active)
            return;

        // Don't hit yourself.
        if (other.transform.root.gameObject == attacker)
            return;


        IDamageable target = other.GetComponent<IDamageable>();

        if (target == null)
            return;

        // Prevent hitting the same enemy 5 times during one swing.
        if (hitTargets.Contains(target))
            return;

        hitTargets.Add(target);

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 direction =
            (other.transform.position - attacker.transform.position).normalized;

        DamageInfo info = new DamageInfo(
            damage,
            hitPoint,
            direction,
            attacker
        );

        target.TakeDamage(info);
    }
}
