using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CastleGate : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has reached the castle gate!");


        }
    }
}
