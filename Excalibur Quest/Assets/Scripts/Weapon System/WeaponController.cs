using UnityEngine;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    public List<Transform> slots = new List<Transform>();
    public int currentWeaponIndex;
    public Weapon currentWeapon;

    private void Awake()
    {
        //SwitchWeapon(0);
    }

    public void EquipWeapon(int index)
    {
        slots[index].GetComponent<Weapon>().isEquipped = true;
        SwitchWeapon(index);
    }

    public void SwitchWeapon(int index)
    {
        currentWeaponIndex = index;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == slots[currentWeaponIndex])
            {
                Weapon weapon = slots[i].GetComponent<Weapon>();

                if (!weapon.isEquipped)
                    continue;

                weapon.gameObject.SetActive(true);
                currentWeapon = weapon;
                continue;
            }

            slots[i].gameObject.SetActive(false);
        }
    }
}
