using System;

using UnityEngine;

public class DriverController {
    
    private readonly TruckController truckController;

    private DrivingInput input;

    public DriverController(TruckController truckController) {
        this.truckController = truckController;
    }

    public void Update() {
        ReadDrivingInput();
        ApplyDrivingInput();
    }

    private void ReadDrivingInput() {
        input = GetDrivingInput();
    }

    private DrivingInput GetDrivingInput() {
        return new DrivingInput {
            gas = Input.GetAxis("Vertical"),
            steering = Input.GetAxis("Horizontal"),
            boost = Input.GetKey(KeyCode.Space),
        };
    }

    private void ApplyDrivingInput() {
        truckController.Drive(input.gas, input.boost);
        truckController.Steer(input.steering);
    }
}