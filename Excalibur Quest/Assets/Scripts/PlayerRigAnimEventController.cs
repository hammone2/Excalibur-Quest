using UnityEngine;
using UnityEngine.Events;

public class PlayerRigAnimEventController : MonoBehaviour
{
    [SerializeField] private BodypartData[] bodyparts;

    public UnityEvent OnStaggerFinished;
    public UnityEvent OnAttackFinished;

    private void Awake()
    {
        SetRagdoll(false);
    }

    public void OnAttackAnimationFinished()
    {
        OnAttackFinished?.Invoke();
    }

    public void OnStaggerAnimationFinished()
    {
        OnStaggerFinished?.Invoke();
    }

    public void SetRagdoll(bool enabled)
    {
        foreach(BodypartData data in bodyparts)
        {
            Transform bone = data.bodypartJointTransform;
            Rigidbody rb = bone.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = !enabled;
            }

            Collider col = bone.GetComponent<Collider>();
            if (col)
            {
                col.enabled = enabled;
            }

            CharacterJoint joint = bone.GetComponent<CharacterJoint>();
            if (joint)
            {
                joint.enablePreprocessing = !enabled;
            }
        }
    }
}
