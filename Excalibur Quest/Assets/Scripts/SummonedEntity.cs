using UnityEngine;

public class SummonedEntity : MonoBehaviour
{
    private Wizard wizard;
    private HealthComponent healthComponent;

    public void Initialize(Wizard wizard)
    {
        this.wizard = wizard;

        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.OnDeath.AddListener(HandleDeath);
        }
    }

    public void HandleDeath()
    {
        if (wizard != null)
        {
            wizard.OnSummonedEnemyDestroyed();
        }
    }
}
