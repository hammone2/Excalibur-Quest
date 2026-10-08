using UnityEngine;
using UnityEngine.Events;

public class OrcRigAnimEventController : MonoBehaviour
{
    public UnityEvent OnStaggerFinished;
    public UnityEvent OnAttackFinished;
    public UnityEvent OnWornOutFinished;
    
    
    public void OrcAttackAnimationFinished()
    {
        OnAttackFinished?.Invoke();
    }

    public void OrcStaggerAnimationFinished()
    {
        OnStaggerFinished?.Invoke();
    }

    public void OrcWornOutAnimationFinished()
    {
        OnWornOutFinished?.Invoke();
    }
}
