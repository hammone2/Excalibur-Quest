using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour
{
    public UnityEvent OnPickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickedUp?.Invoke();
            Destroy(gameObject);
        }
    }
}
