using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleChassie))]
public class UnityVehicle : MonoBehaviour {

    [SerializeField] private float mass = 1000f;

    private Rigidbody physics;
    private VehicleChassie chassie;
    private VehicleDrive drive;
    private VehicleSteeringAxle steeringAxle;
    private VehicleTowing towing;

    public VehicleDrive Drive => drive;
    public VehicleChassie Chassie => chassie;
    public VehicleTowing Towing => towing;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Vector3 Velocity => physics.linearVelocity;

    private void Awake() {
        physics = GetComponent<Rigidbody>();
        chassie = GetComponent<VehicleChassie>();
        drive = GetComponent<VehicleDrive>();
        steeringAxle = GetComponent<VehicleSteeringAxle>();
        towing = GetComponent<VehicleTowing>();
    }

#if UNITY_EDITOR
    private void OnValidate() {
        physics.mass = mass;
    }
#endif

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

    private void FixedUpdate() {
        if (drive != null) {
            chassie.SetAxisfMotorTorque(WheelAxisName.Front, drive.MotorTorque);
            chassie.SetAxisfMotorTorque(WheelAxisName.Rear, drive.MotorTorque);
            chassie.SetAxisSteerAngle(WheelAxisName.Front, drive.SteeringDegree);
        } else if (steeringAxle != null) {
            chassie.SetAxisfMotorTorque(WheelAxisName.Front, steeringAxle.FrontAxisMotorTorque);
            chassie.SetAxisfMotorTorque(WheelAxisName.Rear, steeringAxle.RearAxisMotorTorque);
            chassie.SetAxisSteerAngle(WheelAxisName.Front, steeringAxle.FrontAxisSteeringDegree);
        }
    }

}