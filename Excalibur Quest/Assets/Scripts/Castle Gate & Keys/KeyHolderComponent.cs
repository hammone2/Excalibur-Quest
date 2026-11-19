using UnityEngine;

public class KeyHolderComponent : MonoBehaviour
{
    public int keyCount = 0;

    public void AddKey()
    {
        keyCount++;
        Debug.Log("Key added! Total keys: " + keyCount);
    }
}
