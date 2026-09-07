using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public string attackAnimation;
    private Animator animator;
    [SerializeField] private MeleeHitbox meleeHitbox;

    private void Start()
    {
        animator = PlayerController.Instance.animator;
    }

    public void PlayAttackAnimation()
    {
        animator.Play(attackAnimation, 0, 0f);
        meleeHitbox.BeginAttack(PlayerController.Instance.gameObject);
    }

    public void OnAnimationFinished()
    {
        PlayerController.Instance.EndAttack();
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
