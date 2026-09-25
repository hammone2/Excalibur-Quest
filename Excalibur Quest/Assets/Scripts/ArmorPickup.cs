using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    public void EquipArmor()
    {
        PlayerController.Instance.armorComponent.EquipArmor();
    }
}
