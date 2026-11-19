using UnityEngine;

public class HorseInteraction : MonoBehaviour
{
    private HorseController horse;

    private void Awake()
    {
        horse = GetComponentInParent<HorseController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player != null)
            player.SetNearbyHorse(horse);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player != null)
            player.ClearNearbyHorse(horse);
    }
}