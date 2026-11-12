using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    public void EquipArmor()
    {
        ArmorComponent ac = PlayerController.Instance.armorComponent;

        if (ac.currentArmorCount >= ac.maxArmorCount)
            return;

        ac.EquipArmor();

        GetComponent<ItemPickup>().ShowPickupText();
        SoundFXManager.Instance.PlayRandomSoundFXClip(GetComponent<ItemPickup>().pickupSounds, transform, 1f);
        Destroy(gameObject);
    }
}
