using UnityEngine;
using UnityEngine.Events;

public class KeyHole : MonoBehaviour
{
    [SerializeField] private GameObject[] keyModels;
    [HideInInspector] public UnityEvent OnKeyInserted;
    private bool keyInserted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (keyInserted)
                return;

            if(other.GetComponent<KeyHolderComponent>()?.UseKey() == true)
            {
                int keyIndex = Random.Range(0, keyModels.Length);
                keyModels[keyIndex].SetActive(true);
                OnKeyInserted?.Invoke();
                keyInserted = true;

                Debug.Log("Keys remaining: " + other.GetComponent<KeyHolderComponent>().keyCount.ToString());
            }
        }
    }
}
