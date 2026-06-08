using System;

using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class VehicleDrive : MonoBehaviour {
    
    [SerializeField] private float maxEngineTorque = 1000;
    [SerializeField] private float maxBrakesTorque = 1000;
    [SerializeField] private float maxSteerDegrees = 40f;
    [SerializeField] private float speedCeilingForSteering = 20f;
    [SerializeField] private float minSteerAmount = 0.15f;
    [SerializeField] private float speedKFactor = 2f;
    
    [SerializeField, HideInInspector] private Rigidbody physics;

    internal float MotorTorque { get; private set; }
    internal float BrakesTorque { get; private set; }
    internal float SteeringDegree { get; private set; }

#if UNITY_EDITOR
    private void OnValidate() {
        if (physics == null)
            physics = GetComponent<Rigidbody>();
    }
#endif

    private void Awake() {
        if (physics == null)
            throw new InvalidOperationException();
    }

    public void SetGas(float gas) {
        MotorTorque = gas * maxEngineTorque;
    }

    public void SetBrakes(float brakes) {
        BrakesTorque = brakes * maxBrakesTorque;
    }

    public void SetSteer(float steer) {
        var speed = physics.linearVelocity.magnitude;
        var t = Mathf.Clamp01(speed / speedCeilingForSteering);
        var steerFactor = Mathf.Max(minSteerAmount, 1f - Mathf.Pow(t, speedKFactor));
        var angle = steer * steerFactor * maxSteerDegrees;
        SteeringDegree = angle;
    }

    public void SetSteerAngle(float steerAngle) {
        SteeringDegree = steerAngle;
    }
    
}