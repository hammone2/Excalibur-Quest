using UnityEngine;

public class HitTest : MonoBehaviour, IDamageable
{

    public void TakeDamage(DamageInfo damage)
    {
        // Handle taking damage here
        Debug.Log($"{name} took {damage.amount} damage from {damage.attacker.name} at {damage.hitPoint}");
        HitStop.Instance.Play(0.1f);
    }
}
