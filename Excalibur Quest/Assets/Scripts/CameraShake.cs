using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
    }

    public static void Shake(float strength = 1f)
    {
        if (Instance == null || Instance.impulseSource == null)
        {
            Debug.LogWarning("CameraShake is not present in the scene.");
            return;
        }

        Instance.impulseSource.GenerateImpulseWithForce(strength);
    }

    public static void Shake(Vector3 velocity)
    {
        if (Instance == null || Instance.impulseSource == null)
        {
            Debug.LogWarning("CameraShake is not present in the scene.");
            return;
        }

        Instance.impulseSource.GenerateImpulseWithVelocity(velocity);
    }
}