using System.Collections.Generic;

using UnityEngine;

public class MotorVehicleController {
    
    private readonly MotorVehicleView view;
    private readonly VehicleService vehicleService;
    private readonly SoundManager soundManager;

    private int idCounter;
    private readonly Dictionary<MotorVehicleId, MotorVehicleModel> registry = new ();

    public MotorVehicleController(MotorVehicleView view, VehicleService vehicleService, SoundManager soundManager) {
        this.vehicleService = vehicleService;
        this.soundManager = soundManager;
        this.view = view;
    }

    public void Update() {
        UpdateVehicleOrientation();
        UpdateVehiclePhysics();
        UpdateVehicleSounds();
    }

    public MotorVehicleId SpawnVehicle(Vector3 position, MotorVehicleConfig vehicleConfig) {
        var nextId = new MotorVehicleId(++idCounter);
        var model = new MotorVehicleModel(nextId, position, vehicleConfig);
        registry[nextId] = model;
        model.PhysicsId = vehicleService.CreateVehicle(model.Position, model.PhysicsPrefab);
        model.SoundSourceId = soundManager.StartLoop(model.Position, model.DrivingData.engineIdleSound);
        view.AddVehicle(model.Id, model.Position, model.VisualsPrefab);
        return model.Id;
    }

    public void DeleteVehicle(MotorVehicleId vehicleId) {
        var model = registry[vehicleId];
        vehicleService.DeleteVehicle(model.PhysicsId);
        soundManager.StopLoop(model.SoundSourceId);
        view.RemoveVehicle(model.Id);
        registry.Remove(model.Id);
    }

    public Vector3 GetVehiclePosition(MotorVehicleId id) {
        return registry[id].Position;
    }

    public int ReadVehiclePhysicsId(MotorVehicleId id) {
        return registry[id].PhysicsId;   
    }

    public void BrakeVehicle(MotorVehicleId id, float brakes) {
        var model = registry[id];
        model.BreaksPower = brakes;
    }

    public void DriveVehicle(MotorVehicleId id, float gasInput, bool boost) {
        var model = registry[id];
        model.MotorPower = Throttle(gasInput, Time.deltaTime, boost, model.MotorPower, model.DrivingData);
    }

    private float Throttle(float gas, float deltaTime, bool boost, float lastDrivePower, MotorVehicleConfig.DrivingData drivingData) {
        var maxPower = boost ? 2 : 1;
        var accelerationSpeed = boost ? drivingData.powerAccelerationSpeed * 2 : drivingData.powerAccelerationSpeed;
        
        if (Mathf.Abs(gas) > 0.01f) {
            var targetPower = Mathf.Sign(gas) * maxPower;
            return Mathf.Lerp(lastDrivePower, targetPower, deltaTime * accelerationSpeed);
        } else {
            return 0;
        }
    }

    public void SteerVehicle(MotorVehicleId id, float steerInput) {
        var model = registry[id];
        float t = Mathf.Clamp01(model.PhysicsPose.velocity.magnitude / model.DrivingData.speedCeilingForSteering);
        float steerFactor = 1f - Mathf.Pow(t, model.DrivingData.speedKFactor); // k > 1 makes the falloff sharper near top speed
        var steerLimit = Mathf.Max(model.DrivingData.minStterAmount, steerFactor);
        model.SteeringDegrees = steerInput * steerLimit * model.DrivingData.maxSteerDegrees;
    }

    public void SteerVehicleToward(MotorVehicleId id, Vector3 direction) {
        var model = registry[id];
        var forward = model.PhysicsPose.rotation * Vector3.forward;
        var forwardToDirectionDegrees = Vector3.SignedAngle(forward, direction, Vector3.up);
        model.SteeringDegrees = Mathf.Clamp(forwardToDirectionDegrees, -model.DrivingData.maxSteerDegrees, model.DrivingData.maxSteerDegrees);
    }

    private void UpdateVehiclePhysics() {
        foreach (var model in registry.Values) {
            var physicsId = model.PhysicsId;
            vehicleService.SetVehicleEngineTorque(physicsId, model.MotorPower * model.DrivingData.maxTorque);
            vehicleService.SetVehicleSteer(physicsId, model.SteeringDegrees);
            vehicleService.SetVehicleBreaks(physicsId, model.BreaksPower * model.DrivingData.maxBreaksTorque);
        }
    }

    private void UpdateVehicleSounds() {
        foreach (var model in registry.Values) {
            if (model.SoundSourceId < 0)
                continue;

            var enginePitch = 0.5f + model.MotorPower;
            var engineVolume = 0.5f + model.MotorPower;
            soundManager.UpdateLoop(model.SoundSourceId, model.Position, enginePitch, engineVolume);
        }    
    }

    private void UpdateVehicleOrientation() {
        foreach (var model in registry.Values) {
            model.PhysicsPose = vehicleService.GetVehicleState(model.PhysicsId);
            model.Position = model.PhysicsPose.position;
            view.UpdateVehiclePose(model.Id, model.PhysicsPose);
        }
    }

}