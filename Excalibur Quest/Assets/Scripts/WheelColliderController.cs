
using UnityEngine;

[ExecuteAlways]
public class WheelColliderController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Wheel Settings")]
    public float mass = 20f;
    public float wheelRadius = 0.35f;
    public float wheelDampingRate = 0.25f;
    public float suspensionDistance = 0.2f;
    public float forceAppPointDistance = 0.1f;

    [Header("Suspension Settings")]
    public float spring = 35000f;
    public float damper = 4500f;
    [Range(0f, 1f)]
    public float targetPosition = 0.5f;

    [Header("Forward Friction")]
    public float forwardExtremumSlip = 0.4f;
    public float forwardExtremumValue = 1f;
    public float forwardAsymptoteSlip = 0.8f;
    public float forwardAsymptoteValue = 0.5f;
    public float forwardStiffness = 1f;

    [Header("Sideways Friction")]
    public float sidewaysExtremumSlip = 0.2f;
    public float sidewaysExtremumValue = 1f;
    public float sidewaysAsymptoteSlip = 0.5f;
    public float sidewaysAsymptoteValue = 0.75f;
    public float sidewaysStiffness = 1f;

    private void OnValidate()
    {
        ApplySettings();
    }

    private void Awake()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        WheelCollider[] wheels =
        {
            frontLeft,
            frontRight,
            rearLeft,
            rearRight
        };

        foreach (WheelCollider wheel in wheels)
        {
            if (wheel == null)
                continue;

            wheel.mass = mass;
            wheel.radius = wheelRadius;
            wheel.wheelDampingRate = wheelDampingRate;
            wheel.suspensionDistance = suspensionDistance;
            wheel.forceAppPointDistance = forceAppPointDistance;

            JointSpring suspension = wheel.suspensionSpring;
            suspension.spring = spring;
            suspension.damper = damper;
            suspension.targetPosition = targetPosition;
            wheel.suspensionSpring = suspension;

            WheelFrictionCurve forward = wheel.forwardFriction;
            forward.extremumSlip = forwardExtremumSlip;
            forward.extremumValue = forwardExtremumValue;
            forward.asymptoteSlip = forwardAsymptoteSlip;
            forward.asymptoteValue = forwardAsymptoteValue;
            forward.stiffness = forwardStiffness;
            wheel.forwardFriction = forward;

            WheelFrictionCurve sideways = wheel.sidewaysFriction;
            sideways.extremumSlip = sidewaysExtremumSlip;
            sideways.extremumValue = sidewaysExtremumValue;
            sideways.asymptoteSlip = sidewaysAsymptoteSlip;
            sideways.asymptoteValue = sidewaysAsymptoteValue;
            sideways.stiffness = sidewaysStiffness;
            wheel.sidewaysFriction = sideways;
        }
    }
}
