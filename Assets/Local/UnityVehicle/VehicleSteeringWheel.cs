using System;

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class VehicleSteeringWheel : MonoBehaviour {
    
    [SerializeField] private float maxSteerDegrees = 40f;
    [SerializeField] private float speedCeilingForSteering = 20f;
    [SerializeField] private float minSteerAmount = 0.15f;
    [SerializeField] private float speedKFactor = 2f;

    [SerializeField, HideInInspector] private Rigidbody physics;

    internal float FrontAxisSteeringDegree { get; private set; }

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

    public void SetSteer(float steer) {
        var speed = physics.linearVelocity.magnitude;
        var t = Mathf.Clamp01(speed / speedCeilingForSteering);
        var steerFactor = Mathf.Max(minSteerAmount, 1f - Mathf.Pow(t, speedKFactor));
        var angle = steer * steerFactor * maxSteerDegrees;
        FrontAxisSteeringDegree = angle;
    }

    public void SetSteerAngle(float steerAngle) {
        FrontAxisSteeringDegree = steerAngle;
    }
}