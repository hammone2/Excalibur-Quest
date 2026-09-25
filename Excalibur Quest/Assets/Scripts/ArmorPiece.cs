using UnityEngine;

[System.Serializable]
public class ArmorPiece
{
    [SerializeField] private string armorName; //this is mostly for organizing the armor pieces in the inspector, not for gameplay purposes
    [SerializeField] private Transform[] connectedPieces; //seperate meshes that are connected to the main armor piece
    private bool isEquipped = false;

    public void Equip()
    {
        foreach (Transform piece in connectedPieces)
        {
            piece.gameObject.SetActive(true);
        }
        isEquipped = true;
    }

    public void Deplete()
    {
        foreach (Transform piece in connectedPieces)
        {
            piece.gameObject.SetActive(false);
        }
        isEquipped = false;
    }

    public bool IsEquipped()
    {
        return isEquipped;
    }
}
