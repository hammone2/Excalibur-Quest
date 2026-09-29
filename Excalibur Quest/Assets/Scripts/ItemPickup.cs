using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private bool destroyOnPickup = true;
    public UnityEvent OnPickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickedUp?.Invoke();

            //useful for instances where you run over health potions but your HP is full and you dont want to destroy the pickup
            if (destroyOnPickup) 
                Destroy(gameObject);
        }
    }
}
