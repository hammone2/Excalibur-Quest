using UnityEngine;

public class DismemberableBodyPart : MonoBehaviour
{
    private BodypartData data;
    private bool isDismembered = false;

    public void AssignData(BodypartData data)
    {
        this.data = data;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (data == null) return;

        if (isDismembered) return;

        isDismembered = true;
        
        data.bodypartMesh.SetActive(false);

        GameObject dismemberedPart = Instantiate(data.dismemberedPartPrefab);
        dismemberedPart.transform.position = transform.position;
        dismemberedPart.transform.rotation = transform.rotation;

        //these need to be destroyed/deactivated in this exact order
        Destroy(GetComponent<CharacterJoint>());
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Collider>());
        
    }
}
