using System;

using UnityEngine;

public class DrivingController {
    
    private readonly TruckController truckController;
    private readonly VehicleService vehicleService;

    private DrivingInput input;
    private VehicleState vehicleState;

    public DrivingController(TruckController truckController, VehicleService vehicleService) {
        this.truckController = truckController;
        this.vehicleService = vehicleService;
    }

    public void Update() {
        ReadDrivingInput();
        ReadVehicleState();
        ApplyDrivingInput();
    }

    private void ReadVehicleState() {
        vehicleState = vehicleService.GetVehicleState(truckController.ReadVehiclePhysicsId());
    }

    private void ReadDrivingInput() {
        input = new DrivingInput {
            direction = Input.GetAxis("Vertical"),
            steering = Input.GetAxis("Horizontal"),
            boost = Input.GetKey(KeyCode.Space),
        };
    }

    private void ApplyDrivingInput() {
        float gasThrottle;
        float brakesThrottle;
        if (input.direction > 0) {
            gasThrottle = Mathf.Clamp01(input.direction);
            brakesThrottle = 0;
        } else {
            var forward = vehicleState.rotation * Vector3.forward;
            var velocityTowardFront = Vector3.Dot(forward, vehicleState.velocity) > 0.1;
            if (velocityTowardFront) {
                gasThrottle = 0;
                brakesThrottle = Mathf.Abs(input.direction);
            } else {
                gasThrottle = input.direction;
                brakesThrottle = 0;
            }
        }

        truckController.Drive(gasThrottle, input.boost);
        truckController.Brake(brakesThrottle);
        truckController.Steer(input.steering);
    }
}