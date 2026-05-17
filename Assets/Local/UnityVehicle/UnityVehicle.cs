using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(VehicleBody))]
[RequireComponent(typeof(VehicleChassie))]
public class UnityVehicle : MonoBehaviour {

    private VehicleBody body;
    private VehicleChassie chassie;
    private VehicleDrive drive;

    private void Awake() {
        body = GetComponent<VehicleBody>();
        chassie = GetComponent<VehicleChassie>();
        drive = GetComponent<VehicleDrive>();
    }

    public void Gas(float input) {
        if (drive != null) {
            drive.SetGas(input);
        }
    }

    public void Brakes(float input) {
        if (drive != null) {
            drive.SetBrakes(input);
        }
    }

    public void Steer(float signedInput) {
        if (drive != null) {
            drive.SetSteer(signedInput);
        }
    }

    private void FixedUpdate() {
        if (drive != null) {
            chassie.SetAxisfMotorTorque(WheelAxisName.Front, drive.MotorTorque);
            chassie.SetAxisfMotorTorque(WheelAxisName.Rear, drive.MotorTorque);
            chassie.SetAxisSteerAngle(WheelAxisName.Front, drive.SteeringDegree);
        }
    }

}