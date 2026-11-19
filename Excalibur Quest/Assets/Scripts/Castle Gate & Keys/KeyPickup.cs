using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public void OnPickedUp()
    {
        Debug.Log("Key added to inventory");
        PlayerController.Instance.GetComponent<KeyHolderComponent>().AddKey();
    }
}
