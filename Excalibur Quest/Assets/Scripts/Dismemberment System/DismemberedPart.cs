using UnityEngine;

public class DismemberedPart : MonoBehaviour
{
    private bool alive = true;
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float shrinkSpeed = 3f;
    [SerializeField] private float force = 5f;

    private void Update()
    {
        if (alive)
            return;

        //shrink and destroy
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * shrinkSpeed);

        if (transform.localScale == Vector3.zero)
            Destroy(gameObject); //replace this with a obj pool in the future
    }

    public void StartLifeTimer()
    {
        Invoke(nameof(SetUnAlive), lifeTime);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            ParticleSystem particle = child.GetComponent<ParticleSystem>();
            if (!particle)
                continue;

            particle.Play();
        }

        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f)).normalized;
        GetComponent<Rigidbody>().AddForce(randomDirection * force, ForceMode.Impulse);
    }

    private void SetUnAlive()
    {
        alive = false;
    }
}
