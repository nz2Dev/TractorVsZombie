using System;
using System.Collections.Generic;

using UnityEngine;

public class AssemblingController {

    private readonly PlatformController platformController;
    private readonly TruckController truckController;

    private readonly AssemblingModel model;

    public AssemblingController(PlatformController platformController, TruckController truckController) {
        this.platformController = platformController;
        this.truckController = truckController;
        model = new AssemblingModel();
    }

    public IReadOnlyList<int> ControlledPlatformIds => model.ControlledPlatformIds;

    public void Init(AssemblingPrototype prototype) {
        model.TruckPrototype = prototype.initTruckPrototype;
        SpawnDriver();
        
        model.PickupPlatformPrototype = prototype.pickupPlatformPrototype;

        Vector3 directionStep = prototype.initTruckPrototype.rotation * Vector3.back * 6;
        Vector3 loadoutPosition = prototype.initTruckPrototype.position + directionStep; 
        foreach (var loadout in prototype.initLoadoutPrototypes) {    
            SpawnPlatform(loadoutPosition, out var platformId);
            CouplePlatformToTheEnd(platformId);
            EquipPlatform(platformId, loadout);
            loadoutPosition += directionStep;
        }
    }

    public void AddLoadout(Vector3 position, LoadoutPrototype loadoutPrototype, bool inFrontOrToTheEnd, out PlatformState platformState) {
        SpawnPlatform(position, out var platformId);
        EquipPlatform(platformId, loadoutPrototype);
        platformState = platformController.ReadPlatformState(platformId);
        if (inFrontOrToTheEnd)
            CouplePlatformInFront(platformId);
        else
            CouplePlatformToTheEnd(platformId);
    }

    private void SpawnDriver() {
        truckController.Create(model.TruckPrototype);
    }

    private void SpawnPlatform(Vector3 position, out int platformId) {
        platformId = platformController.Create(model.PickupPlatformPrototype, position);
        model.ControlledPlatformIds.Add(platformId);
    }

    private void EquipPlatform(int platformId, LoadoutPrototype loadout) {
        platformController.SetLoadout(platformId, loadout);
    }

    public void CouplePlatformToTheEnd(int platformId) {
        int targetVehiclePhysicsId;
        if (model.CoupledPlatformIds.Count > 0) {
            var lastPlatformId = model.CoupledPlatformIds[^1];
            targetVehiclePhysicsId = platformController.GetVehiclePhysicsId(lastPlatformId);
        } else {
            targetVehiclePhysicsId = truckController.ReadVehiclePhysicsId();
        }

        platformController.Connect(platformId, targetVehiclePhysicsId);
        model.CoupledPlatformIds.Add(platformId);
    }

    public void CouplePlatformInFront(int platformId) {
        if (model.CoupledPlatformIds.Count > 0) {
            var firstPlatformId = model.CoupledPlatformIds[0];
            platformController.Disconnect(firstPlatformId);

            var newPlatformVehiclePhysicsId = platformController.GetVehiclePhysicsId(platformId);
            platformController.Connect(firstPlatformId, newPlatformVehiclePhysicsId);
        }

        platformController.Connect(platformId, truckController.ReadVehiclePhysicsId());
        model.CoupledPlatformIds.Insert(0, platformId);
    }
}