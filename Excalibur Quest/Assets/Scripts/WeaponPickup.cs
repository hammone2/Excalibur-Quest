using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private int weaponID;
    [SerializeField] private string weaponName;

    public void OnPickupWeapon()
    {
        PlayerController.Instance.weaponController.EquipWeapon(weaponID);
    }
}
