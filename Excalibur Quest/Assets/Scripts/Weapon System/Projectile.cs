using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 direction = (other.transform.position - attacker.transform.position).normalized;

            DamageInfo info = new DamageInfo(
                damage,
            hitPoint,
                direction,
                attacker
            );

            damageable.TakeDamage(10);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }*/
}
