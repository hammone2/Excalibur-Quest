using UnityEngine;

public class Spinnythingy : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private bool randomRotation = true;

    void Start()
    {
        if (randomRotation)
            transform.Rotate(0, Random.Range(0f, 360f), 0);
    }

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
