using UnityEngine;

public class VehicleHurtbox : MonoBehaviour
{
    private float speed => transform.parent.GetComponent<Rigidbody>().linearVelocity.magnitude;
    [SerializeField] private float damageThreshold = 10f;
    [SerializeField] private DamageData damageData;

    private void OnTriggerEnter(Collider other)
    {
        if (speed > damageThreshold)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 direction = (other.transform.position - transform.parent.position).normalized;

                DamageInfo info = new DamageInfo(
                    damageData.damageAmount,
                    hitPoint,
                    direction,
                    transform.parent.gameObject,
                    damageData.hitStopDuration
                );

                damageable.TakeDamage(info);
            }
        }
    }
}
