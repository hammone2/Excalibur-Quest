using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage = 10;
    private float minimumVelocity;
    private GameObject attacker;
    private bool canDamageAttacker = false;
    private float attackerDamageDelay = 1.0f;

    [Header("Explosion Settings")]
    [SerializeField] private bool explosive = false;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 5f;

    [Tooltip("Include all layers containing damageable colliders.")]
    [SerializeField] private LayerMask explosionDamageableLayers = ~0;

    [Tooltip("Include walls, terrain, obstacles, and damageable layers.")]
    [SerializeField] private LayerMask explosionLineOfSightLayers = ~0;

    public void Initialize(int damage, GameObject attacker, float minimumVelocity)
    {
        this.damage = damage;
        this.attacker = attacker;
        this.minimumVelocity = minimumVelocity;
        Invoke(nameof(EnableAttackerDamage), attackerDamageDelay);
    }

    private void EnableAttackerDamage()
    {
        canDamageAttacker = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == attacker && !canDamageAttacker)
        {
            Debug.LogWarning("Projectile collided with its attacker. Ignoring collision.");
            return;
        }

        //Debug.Log("Projectile collided with: " + collision.gameObject.name);

        if (explosive)
        {
            ContactPoint contact = collision.GetContact(0);

            // Move the explosion slightly away from the wall.
            Vector3 explosionPosition = contact.point + contact.normal * 0.05f;

            SpawnExplosion();
            Debug.Log("Projectile exploded on collision with: " + collision.gameObject.name);
            Destroy(gameObject);
            return;
        }

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb)
            {
                //are we too slow to deal damage?
                if (rb.isKinematic == false && rb.linearVelocity.magnitude < minimumVelocity)
                {
                    Debug.LogWarning("Projectile is moving too slowly to deal damage.");
                    return;
                }
            }   

            Vector3 hitPoint = collision.collider.ClosestPoint(transform.position);
            Vector3 direction = (collision.transform.position - attacker.transform.position).normalized;

            DamageInfo info = new DamageInfo(
                damage,
                hitPoint,
                direction,
                attacker
            );

            damageable.TakeDamage(info);
            Destroy(gameObject);
        }
    }

    private void SpawnExplosion()
    {
        if (explosionPrefab == null)
        {
            Debug.LogWarning(
                "Projectile is explosive, but no explosion prefab is assigned."
            );

            return;
        }

        GameObject explosionObject = Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );

        Explosion explosion = explosionObject.GetComponent<Explosion>();

        if (explosion == null)
        {
            Debug.LogError("The assigned explosion prefab does not contain an Explosion component.");

            Destroy(explosionObject);
            return;
        }

        explosion.Initialize(
            damage,
            attacker,
            explosionRadius,
            explosionDamageableLayers,
            explosionLineOfSightLayers,
            true //override the damageable layers to include the attacker if necessary
        );
    }
}
