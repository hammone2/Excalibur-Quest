using UnityEngine;

[RequireComponent(typeof(Projectile))]
public class Fireball : MonoBehaviour
{
    private float speed;
    private float timeAlive;
    private float startTime;
    private Transform target;

    public void Initialize(
        Transform target, 
        float fireballSpeed, 
        int fireballDamage, 
        float timeAlive, 
        GameObject attacker, 
        float minimumVelocity)
    {
        Projectile projectile = GetComponent<Projectile>();
        projectile.Initialize(fireballDamage, attacker, minimumVelocity);

        this.target = target;
        speed = fireballSpeed;
        this.timeAlive = timeAlive;

        startTime = Time.time;
    }

    private void Update()
    {
        bool timeAliveExpired = Time.time - startTime >= timeAlive;
        if (timeAliveExpired)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 adjustedTargetPos = new Vector3(target.position.x, target.position.y + 1, target.position.z);
        transform.position = Vector3.MoveTowards(transform.position, adjustedTargetPos, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            FizzleOut();
        }
    }

    private void FizzleOut()
    {
        //put fizzle out particle here + sound
        Destroy(gameObject);
    }
}
