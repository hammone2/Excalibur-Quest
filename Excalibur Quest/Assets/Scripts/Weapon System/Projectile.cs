using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage = 10;
    private float minimumVelocity;
    private GameObject attacker;

    public void Initialize(int damage, GameObject attacker, float minimumVelocity)
    {
        this.damage = damage;
        this.attacker = attacker;
        this.minimumVelocity = minimumVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == attacker)
        {
            Debug.LogWarning("Projectile collided with its attacker. Ignoring collision.");
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
}
