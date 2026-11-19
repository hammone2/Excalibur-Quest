using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 spinDirectionAndSpeed = new Vector3(0f, 100f, 0f);
    [SerializeField] private Space coordinateSpace = Space.Self;

    void Update()
    {
        // Smoothly rotate the object every frame independent of frame rate
        transform.Rotate(spinDirectionAndSpeed * Time.deltaTime, coordinateSpace);
    }
}