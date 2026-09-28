using System.Collections.Generic;
using UnityEngine;

public class ArmorComponent : MonoBehaviour
{
    [Header("ArmorSlots")]
    [SerializeField] private ArmorPiece[] armorPieces;
    public int maxArmorCount;
    public int currentArmorCount;

    private void Start()
    {
        //disable armor
        for (int i = 0; i < armorPieces.Length; i++)
        {
            maxArmorCount++;
            armorPieces[i].Deplete();
        }
    }

    public void EquipArmor()
    {
        //equip a random armor piece that isnt already equipped
        List<ArmorPiece> unequippedArmor = new List<ArmorPiece>();
        for (int i = 0; i < armorPieces.Length; i++)
        {
            if (!armorPieces[i].IsEquipped())
            {
                unequippedArmor.Add(armorPieces[i]);
                //armorPieces[i].Equip();
                //break;
            }
        }

        if (unequippedArmor.Count > 0)
        {
            int randomIndex = Random.Range(0, unequippedArmor.Count);
            unequippedArmor[randomIndex].Equip();
            currentArmorCount++;
        }
    }
}
