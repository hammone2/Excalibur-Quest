using UnityEngine;

public class VehicleDemoResetter : MonoBehaviour
{
    [SerializeField] private Transform vehicleTransform;
    [SerializeField] private Vector3 resetPosition = new Vector3(0, 0, 0);
    [SerializeField] private Quaternion resetRotation = Quaternion.identity;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetVehicle();
        }
    }
    private void ResetVehicle()
    {
        vehicleTransform.position = resetPosition;
        vehicleTransform.rotation = resetRotation;
        Rigidbody rb = vehicleTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
