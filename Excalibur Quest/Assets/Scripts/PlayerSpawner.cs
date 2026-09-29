using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private CinemachineThirdPersonFollow[] followCams;
    [SerializeField] private LayerMask cameraCollisionLayers;

    private void Start()
    {
        for (int i = 0; i < followCams.Length; i++)
        {
            followCams[i].AvoidObstacles.CollisionFilter = cameraCollisionLayers; 
        }

        transform.DetachChildren();

        Destroy(gameObject);
    }
}
