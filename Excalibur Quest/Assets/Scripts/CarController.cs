using UnityEngine;
using System;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    private InputSystem_Actions inputActions;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;

    //private CarLights carLights;

    public bool ardionoInputEnabled = false;
    private ArduinoInput arduinoInput;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
        arduinoInput = GetComponent<ArduinoInput>();
        ardionoInputEnabled = arduinoInput != null;
    }

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;

        //carLights = GetComponent<CarLights>();
    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
        //WheelEffects();
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    void GetInputs()
    {
        if (!ardionoInputEnabled)
        {
            Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
            moveInput = input.y;
            steerInput = input.x;
            return;
        }

        moveInput = arduinoInput.Speed;
        steerInput = arduinoInput.Steering * arduinoInput.steeringMultiplier * -1;

        Debug.Log($"Arduino Input - Speed: {arduinoInput.Speed}, Steering: {arduinoInput.Speed}");
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 10f * Time.deltaTime);
            }
        }
    }

    void Brake()
    {
        if (inputActions.Player.Jump.IsPressed() || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 60000 * brakeAcceleration * Time.deltaTime;
            }

            //carLights.isBackLightOn = true;
            //carLights.OperateBackLights();
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }

            //carLights.isBackLightOn = false;
            //carLights.OperateBackLights();
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            var dirtParticleMainSettings = wheel.smokeParticle.main;

            if (inputActions.Player.Jump.WasPressedThisFrame() && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true && carRb.linearVelocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}