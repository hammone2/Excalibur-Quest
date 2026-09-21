using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public string attackAnimation;
    [SerializeField] private MeleeHitbox meleeHitbox;
    [SerializeField] private GameObject owner;
    [SerializeField] private Animator animator;
    public bool isEquipped = false;

    public void PlayAttackAnimation()
    {
        animator.Play(attackAnimation, 0, 0f);
        meleeHitbox.BeginAttack(owner);
    }

    public void OnAnimationFinished()
    {
        meleeHitbox.EndAttack();
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
