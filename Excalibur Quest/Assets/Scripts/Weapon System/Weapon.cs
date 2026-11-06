using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public string attackAnimation;
    [SerializeField] private MeleeHitbox meleeHitbox;
    [SerializeField] private GameObject owner;
    [SerializeField] private Animator animator;
    [SerializeField] private bool enemyFriendlyFire = false;
    public bool isEquipped = false;

    public enum WeaponType
    {
        Melee,
        Ranged
    }
    public WeaponType weaponType;

    [Header("Ranged Options")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;

    public void PlayAttackAnimation()
    {
        if (animator)
            animator.Play(attackAnimation, 0, 0f);
        
        if (weaponType == WeaponType.Melee)
        {
            meleeHitbox.BeginAttack(owner, enemyFriendlyFire);
        }
        else if (weaponType == WeaponType.Ranged)
        {
            ShootProjectile();
            Debug.Log("Shooting projectile from weapon: " + weaponName);
        }
    }

    public void OnAnimationFinished()
    {
        if (weaponType == WeaponType.Melee)
        {
            meleeHitbox.EndAttack();
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                //projectileScript.SetOwner(owner);
                //projectileScript.SetFriendlyFire(enemyFriendlyFire);

                projectileScript.Initialize(10, owner, 5f);
                projectile.GetComponent<Rigidbody>().AddForce(muzzle.forward * 100f, ForceMode.Impulse);
            }
        }
    }

    //use these methods for the animation events to enable and disable the hitbox during the attack animation
    public void EnableHitbox()
    {
        meleeHitbox.active = true;
    }

    public void DisableHitbox()
    {
        meleeHitbox.active = false;
    }
    //
}
