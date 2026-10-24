using UnityEngine;

[System.Serializable]
public class BodypartData
{
    public Transform bodypartJointTransform;
    public Transform parentJoint;
    public GameObject bodypartMesh;
    public GameObject dismemberedPartPrefab;
    public bool dismemberable;
}
