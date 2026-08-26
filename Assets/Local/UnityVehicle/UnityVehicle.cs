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
    [SerializeField, ReadOnly] private VehicleDrive drive;
    [SerializeField, ReadOnly] private VehicleSteeringAxle steeringAxle;
    [SerializeField, ReadOnly] private VehicleTowing towing;
    
    [SerializeField, HideInInspector] private Rigidbody physics;

    public VehicleDrive Drive => drive;
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
        
        drive = GetComponent<VehicleDrive>();
        steeringAxle = GetComponent<VehicleSteeringAxle>();
        towing = GetComponent<VehicleTowing>();

        AdjustVehicleMass();
    }
#endif

    private void Awake() {
        if (physics == null || chassie == null)
            throw new InvalidOperationException();

        AdjustVehicleMass();
    }

    private void FixedUpdate() {
        ReadChassieInput();
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

    private void ReadChassieInput() {
        if (drive != null) {
            chassie.SetAxisMotorTorque(WheelAxisName.Front, drive.MotorTorque);
            chassie.SetAxisMotorTorque(WheelAxisName.Rear, drive.MotorTorque);
            chassie.SetAxisBrakesTorque(WheelAxisName.Front, drive.BrakesTorque);
            chassie.SetAxisBrakesTorque(WheelAxisName.Rear, drive.BrakesTorque);
            chassie.SetAxisSteerAngle(WheelAxisName.Front, drive.SteeringDegree);
        } else if (steeringAxle != null) {
            chassie.SetAxisMotorTorque(WheelAxisName.Front, steeringAxle.FrontAxisMotorTorque);
            chassie.SetAxisMotorTorque(WheelAxisName.Rear, steeringAxle.RearAxisMotorTorque);
            chassie.SetAxisSteerAngle(WheelAxisName.Front, steeringAxle.FrontAxisSteeringDegree);
        }
    }
}