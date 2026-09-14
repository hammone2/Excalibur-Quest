using UnityEngine;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    public List<Transform> slots = new List<Transform>();
    public int currentWeaponIndex;
    public Weapon currentWeapon;

    private void Awake()
    {
        SwitchWeapon(0);
    }

    public void SwitchWeapon(int index)
    {
        currentWeaponIndex = index;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == slots[currentWeaponIndex])
            {
                slots[i].gameObject.SetActive(true);
                currentWeapon = slots[i].GetComponent<Weapon>();
                continue;
            }

            slots[i].gameObject.SetActive(false);
        }
    }
}
