using System.Collections.Generic;

using UnityEngine;

public class DriverSimulator {
    
    private int idCounter;
    private Dictionary<int, DriverModel> registry = new();

    public int Create(DriverConfig config) {
        int nextId = ++idCounter;
        var driver = new DriverModel(nextId, config);
        registry[nextId] = driver;
        return nextId;
    }

    public void Update() {
        foreach (var driver in registry.Values) {
            driver.Steer();
            driver.Drive(Time.deltaTime);
            driver.LastOutput = new DriverOutput {
                gasThrottle = driver.GasThrottleOutput,
                breaksThrottle = driver.BrakesThrottleOutput,
                steeringDegrees = driver.SteerDegreesOutput,

                motorTroque = driver.GasThrottleOutput * driver.Config.maxEngineTorque,
                brakesTorque = driver.BrakesThrottleOutput * driver.Config.maxBrakesTorque
            };
        }
    }

    public void SetInput(int driverId, float driveInput, float steerInput, bool boostInput) {
        var driver = registry[driverId];
        driver.SteerByAmountOrDirection = true;
        driver.DriveAmountInput = driveInput;
        driver.SteerAmountInput = steerInput;
        driver.BoostInput = boostInput;
    }

    public void SetVehicleInput(int driverId, Vector3 vehicleVelocity) {
        var driver = registry[driverId];
        driver.VelocityInput = vehicleVelocity;
    }

    public DriverOutput GetOutput(int driverId) {
        return registry[driverId].LastOutput;
    }

}