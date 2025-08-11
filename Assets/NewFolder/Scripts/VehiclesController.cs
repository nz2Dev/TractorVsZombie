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
        SpawnDriveVehicle();

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
        combatService.UpdateAgentPosition(driveVehicleCombatId, driveVehicle.Position);
        if (combatService.ApplyPushDamage(driveVehicleCombatId, Vector3.one)) {
            Debug.Log("Vehicle applied push damage");
        }
    }

    private void SpawnDriveVehicle() {
        var driveVehiclePosition = Vector3.zero;
        driveVehicle = new Vehicle(driveVehicleBlueprint.wheelAxisDatas.Length, driveVehicleBlueprint.towingWheel);
        vehicles.Add(driveVehicle);
        vehicleService.CreateVehicle(driveVehicleBlueprint.baseSize, driveVehicleBlueprint.wheelAxisDatas, driveVehicleBlueprint.GetTowingWheelAxisData(), mass: driveVehicleBlueprint.mass);
        vehicleView.AddVehicle(driveVehiclePosition, driveVehicleBlueprint.baseGeometry, driveVehicleBlueprint.wheelGeometry, driveVehicleBlueprint.towingBodyGeometry, driveVehicleBlueprint.wheelAxisDatas, driveVehicleBlueprint.GetTowingWheelAxisData());
        
        driveVehicleCombatId = combatService.RegisterCombatant(1, driveVehiclePosition, 10);
    }

    private void SpawnTrailerVehicle(Vector3 position) {
        var trailerVehiclePosition = position;
        var vehicle = new Vehicle(trailerVehicleBlueprint.wheelAxisDatas.Length, trailerVehicleBlueprint.towingWheel);
        vehicles.Add(vehicle);
        vehicleService.CreateVehicle(trailerVehicleBlueprint.baseSize, trailerVehicleBlueprint.wheelAxisDatas, trailerVehicleBlueprint.GetTowingWheelAxisData(), trailerVehiclePosition, mass: driveVehicleBlueprint.mass);
        vehicleView.AddVehicle(trailerVehiclePosition, trailerVehicleBlueprint.baseGeometry, trailerVehicleBlueprint.wheelGeometry, trailerVehicleBlueprint.towingBodyGeometry, trailerVehicleBlueprint.wheelAxisDatas, trailerVehicleBlueprint.GetTowingWheelAxisData());

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
            vehicle.Orient(vehiclePose.position, vehiclePose.rotation);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                var wheelAxisPose = vehicleService.GetVehicleWheelAxisPose(vehiclePhysicsRigIndex, wheelAxisIndex);
                vehicle.OrientWheelAxis(wheelAxisIndex, wheelAxisPose);
            }   

            if (vehicle.TowingWheelAxisPose.HasValue) {
                var towingWheelAxisPose = vehicleService.GetVehicleTowingWheelAxisPose(vehiclePhysicsRigIndex);
                vehicle.OrientTowingWheelAxis(towingWheelAxisPose);
            }
        }
    }

    private void UpdateVehiclesView() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicleViewIndex = vehicleIndex;
            vehicleView.UpdateVehiclePose(vehicleViewIndex, new VehiclePose {
                position = vehicle.Position,
                rotation = vehicle.Rotation
            });

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                vehicleView.UpdateWheelAxisPose(vehicleViewIndex, wheelAxisIndex, vehicle.WheelAxisPoses[wheelAxisIndex]);
            }   

            if (vehicle.TowingWheelAxisPose.HasValue) {
                vehicleView.UpdateTowingWheelAxisPose(vehicleViewIndex, vehicle.TowingWheelAxisPose.Value);
            }
        }
    }
}