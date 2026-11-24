using UnityEngine;

public class KeyHolderComponent : MonoBehaviour
{
    public int keyCount = 0; //the current amount of keys in the player's possession
    public int keysCollected = 0; //the total that have been collected/inserted into the castle gate

    public void AddKey()
    {
        keyCount++;
        keysCollected++;
        Debug.Log("Key added! Total keys: " + keyCount);
    }

    public bool UseKey()
    {
        if (keyCount > 0)
        {
            keyCount--;
            Debug.Log("Key used! Remaining keys: " + keyCount);
            return true;
        }

        Debug.Log("No keys available to use!");
        return false;
    }
}
