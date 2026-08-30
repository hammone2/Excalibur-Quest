using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public string attackAnimation;
    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation()
    {
        animator.Play(attackAnimation, 0, 0f);
    }

    public void OnAnimationFinished()
    {
        PlayerController.Instance.EndAttack();
    }
}
