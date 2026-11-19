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

        GetComponent<ItemPickup>().ShowPickupText();
        SoundFXManager.Instance.PlayRandomSoundFXClip(GetComponent<ItemPickup>().pickupSounds, transform, 1f);
        Destroy(gameObject);
    }
}
