using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Bootstrapper : MonoBehaviour {

    [Range(1, 50)][SerializeField] private float maxTorque = 49;
    [Range(1, 45)][SerializeField] private float maxSteerAngel = 45;

    private DriveVehicle driveVehicle;
    private TrailerVehicle trailerVehicle;

    void Start() {
        driveVehicle = new DriveVehicle();
        trailerVehicle = new TrailerVehicle();
        trailerVehicle.Connect(driveVehicle.Rigidbody, new Vector3(0, 0, -0.7f));
    }

    private void Update() {
        var verticalAxisInput = Input.GetAxis("Vertical");

        if (verticalAxisInput > 0) {
            driveVehicle.Gas(verticalAxisInput * maxTorque);
            driveVehicle.Brakes(0);
        } else {
            driveVehicle.Gas(0);
            driveVehicle.Brakes(-verticalAxisInput * maxTorque);
        }

        var horizontalAxisInput = Input.GetAxis("Horizontal");
        driveVehicle.Steer(horizontalAxisInput * maxSteerAngel);
    }

}
