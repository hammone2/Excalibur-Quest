using Unity.VisualScripting;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private BodypartData[] bodyparts;

    private void Awake()
    {
        SetRagdoll(false);
    }

    public void SetRagdoll(bool enabled)
    {
        foreach (BodypartData data in bodyparts)
        {

            Transform bone = data.bodypartJointTransform;

            if (!enabled) //ragdoll is being initialized. Assign dismemberable class to appropriate parts.
            {
                if (data.dismemberable)
                {
                    bone.AddComponent<DismemberableBodyPart>();
                    DismemberableBodyPart dismemberableBodyPart = bone.GetComponent<DismemberableBodyPart>();

                    dismemberableBodyPart.AssignData(data);
                }
            }

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
