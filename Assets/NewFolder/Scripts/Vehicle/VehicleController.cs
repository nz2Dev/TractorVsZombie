using System.Collections.Generic;

using UnityEngine;

public class VehicleController {

    private readonly VehicleView view;
    private readonly CombatService combatService;
    private readonly VehicleService vehicleService;
    private readonly SoundManager soundManager;

    private int idCounter;
    private Dictionary<int, VehicleModel> registry = new ();

    public VehicleController(VehicleService vehicleService, CombatService combatService, SoundManager soundManager, VehicleView view) {
        this.vehicleService = vehicleService;
        this.combatService = combatService;
        this.soundManager = soundManager;
        this.view = view;
    }

    public void Update() {
        UpdateVehicleOrientation();
        UpdateVehiclePhysics();
        UpdateVehicleSounds();
        ComputeRamDamage();
    }

    public int SpawnVehicle(Vector3 position, int ramCombatId, VehicleConfig vehicleConfig) {
        var nextId = ++idCounter;
        var model = new VehicleModel(nextId, position, vehicleConfig);
        registry[nextId] = model;
        model.RamCombatId = ramCombatId;
        model.PhysicsId = vehicleService.CreateVehicle(model.Position, model.PhysicsPrefab);
        if (model.EngineIdleSound != null) {
            model.SoundSourceId = soundManager.StartLoop(model.Position, model.EngineIdleSound);
        }
        view.AddVehicle(model.Id, model.Position, model.VisualsPrefab);
        return model.Id;
    }

    public void DeleteVehicle(int vehicleId) {
        var model = registry[vehicleId];
        vehicleService.DeleteVehicle(model.PhysicsId);
        if (model.EngineIdleSound != null) {
            soundManager.StopLoop(model.SoundSourceId);
        }
        view.RemoveVehicle(model.Id);
        registry.Remove(model.Id);
    }

    public Vector3 GetVehiclePosition(int vehicleId) {
        return registry[vehicleId].Position;
    }

    public void BrakeVehicle(int vehicleId, float brakes) {
        var model = registry[vehicleId];
        model.BreaksPower = brakes;
    }

    public void DriveVehicle(int vehicleId, float gasInput, bool boost) {
        var model = registry[vehicleId];
        model.MotorPower = Throttle(gasInput, Time.deltaTime, boost, model.MotorPower, model.DrivingData);
    }

    private float Throttle(float gas, float deltaTime, bool boost, float lastDrivePower, DrivingData drivingData) {
        var maxPower = boost ? 2 : 1;
        var accelerationSpeed = boost ? drivingData.powerAccelerationSpeed * 2 : drivingData.powerAccelerationSpeed;
        if (gas > 0) {
            return Mathf.Lerp(lastDrivePower, maxPower, deltaTime * accelerationSpeed);
        } else {
            return 0;
        }
    }

    public void SteerVehicle(int vehicleId, float steerInput) {
        var model = registry[vehicleId];
        float t = Mathf.Clamp01(model.Velocity.magnitude / model.DrivingData.speedCeilingForSteering);
        float steerFactor = 1f - Mathf.Pow(t, model.DrivingData.speedKFactor); // k > 1 makes the falloff sharper near top speed
        var steerLimit = Mathf.Max(model.DrivingData.minStterAmount, steerFactor);
        model.SteeringDegrees = steerInput * steerLimit * model.DrivingData.maxSteerDegrees;
    }

    public void SteerVehicleToward(int vehicleId, Vector3 direction) {
        var model = registry[vehicleId];
        var forward = model.Rotation * Vector3.forward;
        var forwardToDirectionDegrees = Vector3.SignedAngle(forward, direction, Vector3.up);
        model.SteeringDegrees = Mathf.Clamp(forwardToDirectionDegrees, -model.DrivingData.maxSteerDegrees, model.DrivingData.maxSteerDegrees);
    }

    public void ConnectVehicles(int headVehicleId, int tailVehicleId) {
        var headModel = registry[headVehicleId];
        var tailModel = registry[tailVehicleId];
        vehicleService.MakeTowingConnection(headModel.PhysicsId, tailModel.PhysicsId);
    }

    public void DisconnectVehicle(int vehicleId) {
        var vehicle = registry[vehicleId];
        vehicleService.ClearTowingConnection(vehicle.PhysicsId);
    }

    private void ComputeRamDamage() {
        foreach (var vehicle in registry.Values) {
            if (!vehicle.RamData.enabled)
                continue;

            var affectedCount = combatService.ApplyExplosionDamage(vehicle.RamCombatId, vehicle.Position, vehicle.RamData.radius, damage: 0);
            for (int i = 0; i < affectedCount; i++) {
                var position = vehicle.Position + Random.onUnitSphere * vehicle.RamData.radius;
                soundManager.PlayEffectDelayed(position, i * 0.05f, vehicle.RamData.impactSFX);
            }    
        }
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
            view.UpdateVehiclePose(model.Id, model.PhysicsPose);
        }
    }

}