using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Bootstrapper : MonoBehaviour {

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
            driveVehicle.Gas(verticalAxisInput * 50);
            driveVehicle.Brakes(0);
        } else {
            driveVehicle.Gas(0);
            driveVehicle.Brakes(-verticalAxisInput * 50);
        }

        var horizontalAxisInput = Input.GetAxis("Horizontal");
    }

}
