using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private GameObject[] keyModels;

    private void Start()
    {
        if (keyModels.Length > 0)
        {
            int randomIndex = Random.Range(0, keyModels.Length);
            for (int i = 0; i < keyModels.Length; i++)
            {
                if (i == randomIndex)
                {
                    keyModels[i].SetActive(true);
                }
                else
                {
                    keyModels[i].SetActive(false);
                }
            }
        }
    }

    public void OnPickedUp()
    {
        Debug.Log("Key added to inventory");
        PlayerController.Instance.GetComponent<KeyHolderComponent>().AddKey();
    }
}
