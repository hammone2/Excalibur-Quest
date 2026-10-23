using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage = 10;
    private GameObject attacker;

    public void Initialize(int damage, GameObject attacker)
    {
        this.damage = damage;
        this.attacker = attacker;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (GetComponent<Rigidbody>().linearVelocity.magnitude < 1f)
                return;

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
