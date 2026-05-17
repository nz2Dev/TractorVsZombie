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

    private void Awake() {
        physics = GetComponent<Rigidbody>();
        chassie = GetComponent<VehicleChassie>();
        drive = GetComponent<VehicleDrive>();
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
        }
    }

}