using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    public void EquipArmor()
    {
        ArmorComponent ac = PlayerController.Instance.armorComponent;

        if (ac.currentArmorCount >= ac.maxArmorCount)
            return;

        ac.EquipArmor();

        Destroy(gameObject);
    }
}
