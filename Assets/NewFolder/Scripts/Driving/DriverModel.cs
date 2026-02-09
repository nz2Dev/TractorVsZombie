using UnityEngine;

public class DriverModel {
    
    public DriverConfig Config { get; }
    public int Id { get; }
    public bool SteerByAmountOrDirection { get; set; }

    public float DriveAmountInput { get; set; }
    public float SteerAmountInput { get; set; }
    public Vector3 SteerDirectionInput { get; set; }
    public bool BoostInput { get; set; }
    
    public Vector3 VelocityInput { get; set; }
    public DriverOutput LastOutput { get; set; }

    public float GasThrottleOutput { get; set; }
    public float BrakesThrottleOutput { get; set; }
    public float SteerDegreesOutput { get; set; }

    public DriverModel(int id, DriverConfig config) {
        Id = id;
        this.Config = config;
    }

    internal void Drive(float deltaTime) {
        var powerMultiplier = BoostInput ? 2 : 1;
        var accelerationMultiplier = BoostInput ? 2 : 1;
        
        BrakesThrottleOutput = 0;
        if (Mathf.Abs(DriveAmountInput) > 0.01f) {
            var targetPower = Mathf.Sign(DriveAmountInput) * powerMultiplier;
            GasThrottleOutput = Mathf.Lerp(LastOutput.gasThrottle, targetPower, deltaTime * accelerationMultiplier * Config.powerAccelerationSpeed);
        } else {
            GasThrottleOutput = 0;
        }
    }

    internal void Steer() {
        if (SteerByAmountOrDirection) {
            SteerAmount();
        } else {
            SteerToward();
        }
    }

    internal void SteerAmount() {
        float t = Mathf.Clamp01(VelocityInput.magnitude / Config.speedCeilingForSteering);
        float steerFactor = 1f - Mathf.Pow(t, Config.speedKFactor); // k > 1 makes the falloff sharper near top speed
        var steerLimit = Mathf.Max(Config.minStterAmount, steerFactor);
        SteerDegreesOutput = SteerAmountInput * steerLimit * Config.maxSteerDegrees;
    }

    internal void SteerToward() {
        var forward = VelocityInput;
        var forwardToDirectionDegrees = Vector3.SignedAngle(forward, SteerDirectionInput, Vector3.up);
        SteerDegreesOutput = Mathf.Clamp(forwardToDirectionDegrees, -Config.maxSteerDegrees, Config.maxSteerDegrees);
    }

}