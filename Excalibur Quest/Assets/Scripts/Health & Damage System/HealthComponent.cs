using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour, IDamageable
{
    public int healthPoints = 3;

    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;

    public void TakeDamage(DamageInfo damage)
    {
        if (healthPoints <= 0) return;

        Debug.Log($"{name} took {damage.amount} damage from {damage.attacker.name} at {damage.hitPoint}");
        HitStop.Instance.Play(0.1f);

        healthPoints -= damage.amount;

        if (healthPoints <= 0)
        {
            OnDeath?.Invoke();
            return;
        }

        OnTakeDamage?.Invoke();
    }

    public void Heal(int amount)
    {
        healthPoints += amount;
        Debug.Log($"{name} healed {amount} points. Current health: {healthPoints}");
    }
}
