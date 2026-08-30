using System;

using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleChassie))]
public class UnityVehicle : MonoBehaviour {

    [SerializeField] private float mass = 1000f;
    [Space]
    [SerializeField, ReadOnly] private VehicleChassie chassie;
    [SerializeField, ReadOnly] private VehiclePowertrain powertrain;
    [SerializeField, ReadOnly] private VehicleSteeringWheel steeringWheel;
    [SerializeField, ReadOnly] private VehicleSteeringAxle steeringAxle;
    [SerializeField, ReadOnly] private VehicleTowing towing;
    
    [SerializeField, HideInInspector] private Rigidbody physics;

    public VehiclePowertrain Powertrain => powertrain;
    public VehicleSteeringWheel SteeringWheel => steeringWheel;
    public VehicleChassie Chassie => chassie;
    public VehicleTowing Towing => towing;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Vector3 Velocity => physics.linearVelocity;

#if UNITY_EDITOR
    private void OnValidate() {
        if (physics == null)
            physics = GetComponent<Rigidbody>();
        if (chassie == null)
            chassie = GetComponent<VehicleChassie>();
        
        powertrain = GetComponent<VehiclePowertrain>();
        steeringWheel = GetComponent<VehicleSteeringWheel>();
        steeringAxle = GetComponent<VehicleSteeringAxle>();
        towing = GetComponent<VehicleTowing>();

        if (steeringWheel != null && steeringAxle != null)
            Debug.LogWarning($"both {steeringWheel} and {steeringAxle} exist");

        AdjustVehicleMass();
    }
#endif

    private void Awake() {
        if (physics == null || chassie == null)
            throw new InvalidOperationException();

        if (steeringWheel != null && steeringAxle != null)
            throw new InvalidOperationException($"steering component conflict {steeringWheel} and {steeringAxle}");

        AdjustVehicleMass();
    }

    private void FixedUpdate() {
        if (powertrain != null) {
            chassie.SetAxisMotorTorque(WheelAxisName.Front, powertrain.MotorTorque);
            chassie.SetAxisMotorTorque(WheelAxisName.Rear, powertrain.MotorTorque);
            chassie.SetAxisBrakesTorque(WheelAxisName.Front, powertrain.BrakesTorque);
            chassie.SetAxisBrakesTorque(WheelAxisName.Rear, powertrain.BrakesTorque);
        } else if (towing != null) {
            chassie.SetAxisMotorTorque(WheelAxisName.Front, 1f);
            chassie.SetAxisMotorTorque(WheelAxisName.Rear, -1f);
        }
        
        if (steeringWheel != null) {
            chassie.SetAxisSteerAngle(WheelAxisName.Front, steeringWheel.FrontAxisSteeringDegree);
        } else if (steeringAxle != null) {
            chassie.SetAxisSteerAngle(WheelAxisName.Front, steeringAxle.FrontAxisSteeringDegree);
        }
    }

    public void Transform(Vector3 position, Quaternion rotation) {
        transform.SetPositionAndRotation(position, rotation);
        physics.position = position;
        physics.rotation = rotation;
    }

    public void DestroySelf() {
        chassie.baseCollider.isTrigger = true;
        physics.isKinematic = true;
        physics.linearVelocity = Vector3.zero;
        physics.angularVelocity = Vector3.zero;
    }

    private void AdjustVehicleMass() {
        physics.mass = mass;
    }
    
}