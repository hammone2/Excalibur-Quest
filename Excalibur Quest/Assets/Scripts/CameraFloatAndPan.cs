using UnityEngine;

public class CameraFloatAndPan : MonoBehaviour
{
    public float floatAmount = 0.1f; // Amount of floating
    public float floatInterval = 2f; // Time interval for floating up/down

    public float panRange = 0.1f; // Maximum distance to pan
    public float panInterval = 2f; //Time interval for panning 

    public float offset = 0.0f; // phase shift

    private float startYPos;
    private float startYRot;

    private void Start()
    {
        // doing this so the camera's origin is clamped to its original y position/rotation.
        startYPos = transform.localPosition.y;
        startYRot = transform.localRotation.eulerAngles.y;
    }

    void Update()
    {
        // Calculate the new Y position based on the sine wave
        float newYPos = floatAmount * Mathf.Sin(Time.time * floatInterval + offset);
        float newYRot = panRange * Mathf.Sin(Time.time * panInterval + offset);

        // Update the position of the GameObject
        transform.localPosition = new Vector3(transform.localPosition.x, startYPos + newYPos, transform.localPosition.z);
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, startYRot + newYRot, transform.localRotation.z);
    }
}