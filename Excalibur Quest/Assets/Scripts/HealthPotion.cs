using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private int healthAmount = 25;

    public void OnPickedUp()
    {
        HealthComponent hc = PlayerController.Instance.GetComponent<HealthComponent>();
        if (hc == null)
            return;

        if (hc.healthPoints >= hc.maxHealth)
            return;

        hc.Heal(healthAmount);
        Destroy(gameObject);
    }
}
