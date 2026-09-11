using UnityEngine;

public class DismemberedPart : MonoBehaviour
{
    private bool alive = true;
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float shrinkSpeed = 3f;

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
    }

    private void SetUnAlive()
    {
        alive = false;
    }
}
