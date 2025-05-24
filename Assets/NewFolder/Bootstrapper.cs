using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class Bootstrapper : MonoBehaviour {

    [Range(1, 1000)][SerializeField] private float maxTorque = 49;
    [Range(1, 45)][SerializeField] private float maxSteerAngel = 45;
    [SerializeField] private bool autoDrive = false;
    [SerializeField] private RigidbodyInterpolation interpolationMode = RigidbodyInterpolation.Interpolate;
    [SerializeField] public int targetFrameRate = 120;
    [SerializeField] public int vSyncCount = 0;

    private DriveVehicle driveVehicle;

    private void Awake() {
        QualitySettings.vSyncCount = vSyncCount;
        Application.targetFrameRate = targetFrameRate;
    }

    void Start() {
        var connectionOffset = new Vector3(0, 0, -0.7f);
        driveVehicle = new DriveVehicle(maxTorque);
        driveVehicle.Rigidbody.interpolation = interpolationMode;
        
        var trailerVehicle = new TrailerVehicle();
        trailerVehicle.Connect(driveVehicle.Rigidbody, connectionOffset);
        trailerVehicle.Rigidbody.interpolation = interpolationMode;

        var trailerVehicle2 = new TrailerVehicle();
        trailerVehicle2.Connect(trailerVehicle, connectionOffset);
        trailerVehicle2.Rigidbody.interpolation = interpolationMode;
    }

    private void Update() {
        var verticalAxisInput = Input.GetAxis("Vertical");
        var horizontalAxisInput = Input.GetAxis("Horizontal");
        if (autoDrive) {
            verticalAxisInput = 1;
            horizontalAxisInput = 1;
        }

        if (verticalAxisInput > 0) {
            driveVehicle.Throttle(verticalAxisInput);
            driveVehicle.Brakes(0);
        } else {
            driveVehicle.Throttle(0);
            driveVehicle.Brakes(-verticalAxisInput);
        }

        driveVehicle.Steer(horizontalAxisInput * maxSteerAngel);
    }

}
