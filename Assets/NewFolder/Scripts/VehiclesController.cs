using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class VehiclesController : MonoBehaviour {
    
    private readonly int trailersCount;
    private readonly VehicleBlueprint driveVehicleBlueprint;
    private readonly VehicleBlueprint trailerVehicleBlueprint;

    private readonly CombatService combatService;
    private readonly VehicleService vehicleService;
    private readonly VehicleView vehicleView;

    private readonly List<Vehicle> vehicles = new ();
    private Vehicle driveVehicle;
    private int driveVehicleCombatId;

    public VehiclesController(VehicleService vehicleService, VehicleView vehicleView, VehicleBlueprint driveVehicle, VehicleBlueprint trailerVehicle, int trailersCount, CombatService combatService) {
        this.vehicleService = vehicleService;
        this.vehicleView = vehicleView;
        this.driveVehicleBlueprint = driveVehicle;
        this.trailerVehicleBlueprint = trailerVehicle;
        this.trailersCount = trailersCount;
        this.combatService = combatService;
    }

    public void Init() {
        SpawnDriveVehicle(Vector3.zero);

        for (int i = 0; i < trailersCount; i++) {
            SpawnTrailerVehicle(new Vector3(0, 0, -2f + i * -2f));
        }
    }

    public void Update() {
        ReadVehiclesOrientation();
        ReadDriveVehicleInput();
        UpdateVehiclesView();
        UpdateVehicleCombat();
    }

    private void ReadDriveVehicleInput() {
        const float maxSteerAngle = 35;
        var gasInput = Input.GetAxis("Vertical");
        var steerInput = Input.GetAxis("Horizontal");

        vehicleService.SetVehicleGasThrottle(vehicleIndex: 0, gasInput);
        vehicleService.SetVehicleSteer(vehicleIndex: 0, steerInput * maxSteerAngle);
    }

    private void UpdateVehicleCombat() {
        combatService.UpdateAgentPosition(driveVehicleCombatId, driveVehicle.BodyPose.position);
        if (combatService.ApplyPushDamage(driveVehicleCombatId, Vector3.one)) {
            Debug.Log("Vehicle applied push damage");
        }
    }

    private void SpawnDriveVehicle(Vector3 driveVehiclePosition) {
        driveVehicle = new Vehicle(driveVehicleBlueprint.physicsData);
        vehicleService.CreateVehicle(driveVehiclePosition, driveVehicleBlueprint.physicsData);
        vehicleView.AddVehicle(driveVehiclePosition, driveVehicleBlueprint.physicsData, driveVehicleBlueprint.visualsId);
        
        vehicles.Add(driveVehicle);
        driveVehicleCombatId = combatService.RegisterCombatant(1, driveVehiclePosition, 10);
    }

    private void SpawnTrailerVehicle(Vector3 position) {
        var trailerVehicle = new Vehicle(trailerVehicleBlueprint.physicsData);
        vehicleService.CreateVehicle(position, trailerVehicleBlueprint.physicsData);
        vehicleView.AddVehicle(position, trailerVehicleBlueprint.physicsData, trailerVehicleBlueprint.visualsId);

        vehicles.Add(trailerVehicle);
        var lastIndex = vehicles.Count - 1;
        vehicleService.MakeTowingConnection(
            headVehicleIndex: lastIndex - 1, 
            tailVehicleIndex: lastIndex);
    }

    private void ReadVehiclesOrientation() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehiclePhysicsRigIndex = vehicleIndex;
            
            var vehiclePose = vehicleService.GetVehiclePose(vehiclePhysicsRigIndex);
            vehicle.OrientBody(vehiclePose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                var wheelAxisPose = vehicleService.GetVehicleWheelAxisPose(vehiclePhysicsRigIndex, wheelAxisIndex);
                vehicle.OrientWheelAxis(wheelAxisIndex, wheelAxisPose);
            }   

            if (vehicle.TowingTonqueRotation.HasValue) {
                var towingTonguePose = vehicleService.GetTowingTonguePose(vehiclePhysicsRigIndex);
                vehicle.OrientTowingTonque(towingTonguePose);
            }
        }
    }

    private void UpdateVehiclesView() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicleViewIndex = vehicleIndex;
            vehicleView.UpdateVehiclePose(vehicleViewIndex, vehicle.BodyPose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                vehicleView.UpdateWheelAxisPose(vehicleViewIndex, wheelAxisIndex, vehicle.WheelAxisPoses[wheelAxisIndex]);
            }   

            if (vehicle.TowingTonqueRotation.HasValue) {
                vehicleView.UpdateTowingTonguePose(vehicleViewIndex, vehicle.TowingTonqueRotation.Value);
            }
        }
    }
}