using UnityEngine;

public class PlayerRigAnimEventController : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;

    public void OnAttackAnimationFinished()
    {
        if (weaponController)
            weaponController.currentWeapon.OnAnimationFinished();
    }
}
