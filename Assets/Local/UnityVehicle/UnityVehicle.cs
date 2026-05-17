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

    private void Awake() {
        physics = GetComponent<Rigidbody>();
        chassie = GetComponent<VehicleChassie>();
        drive = GetComponent<VehicleDrive>();
        steeringAxle = GetComponent<VehicleSteeringAxle>();
    }

#if UNITY_EDITOR
    private void OnValidate() {
        physics.mass = mass;
    }
#endif

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