using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Bootstrapper : MonoBehaviour {

    [Range(1, 50)][SerializeField] private float maxTorque = 49;
    [Range(1, 45)][SerializeField] private float maxSteerAngel = 45;

    private DriveVehicle driveVehicle;

    void Start() {
        Application.targetFrameRate = 60;
        var connectionOffset = new Vector3(0, 0, -0.7f);
        driveVehicle = new DriveVehicle();
        var trailerVehicle = new TrailerVehicle();
        trailerVehicle.Connect(driveVehicle.Rigidbody, connectionOffset);
        var trailerVehicle2 = new TrailerVehicle();
        trailerVehicle2.Connect(trailerVehicle, connectionOffset);
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
