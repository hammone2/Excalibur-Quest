using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float height = 0.5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool randomHover = true;
    private float startingHeight;

    private void Start()
    {
        startingHeight = transform.position.y;

        if (randomHover)
            transform.position = new Vector3(transform.position.x, startingHeight + Random.Range(-height, height), transform.position.z);
    }

    private void Update()
    {
        float hover = Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(transform.position.x, startingHeight + hover, transform.position.z);
    }
}
