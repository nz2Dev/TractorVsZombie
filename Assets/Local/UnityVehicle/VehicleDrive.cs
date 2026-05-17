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

    private Rigidbody physics;

    internal float MotorTorque { get; private set; }
    internal float BrakesTorque { get; private set; }
    internal float SteeringDegree { get; private set; }

    private void Awake() {
        physics = GetComponent<Rigidbody>();
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

}