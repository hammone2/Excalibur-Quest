using UnityEngine;

public class FlyingMachineController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private float altitudeSpeed = 5f;

    private InputSystem_Actions inputActions;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputSystem_Actions();
        inputActions.FlyingMachine.Enable();
    }

    private void OnDestroy()
    {
        inputActions.FlyingMachine.Disable();
    }

    private void FixedUpdate()
    {
        float accelerationInput = inputActions.FlyingMachine.Accelerate.ReadValue<float>();
        float turnInput = inputActions.FlyingMachine.Turn.ReadValue<float>();
        float altitudeInput = inputActions.FlyingMachine.Altitude.ReadValue<float>();


        //Turning
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float torqueAmount = turnInput * turnSpeed * Mathf.Deg2Rad;
            Vector3 torque = Vector3.up * torqueAmount;
            rb.AddTorque(torque, ForceMode.Force);
        }

        // Apply forward force based on input
        if (Mathf.Abs(accelerationInput) > 0.01f)
        {
            float desiredSpeed = accelerationInput * acceleration;
            desiredSpeed = Mathf.Clamp(desiredSpeed, -maxSpeed, maxSpeed);
            Vector3 forwardForce = rb.rotation * Vector3.forward * (desiredSpeed);
            rb.AddForce(forwardForce, ForceMode.Force);
        }

        // Apply upward force if there is altitude input
        if (Mathf.Abs(altitudeInput) > 0.01f)
        {
            Vector3 upwardForce = Vector3.up * (altitudeInput * altitudeSpeed);
            rb.AddForce(upwardForce, ForceMode.Force);
        }
    }
}
