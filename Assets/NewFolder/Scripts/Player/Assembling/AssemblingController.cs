using System;
using System.Collections.Generic;

using UnityEngine;

public class AssemblingController {

    private readonly PlatformController platformController;
    private readonly TruckController truckController;

    private readonly AssemblingModel model;

    public event Action<int> OnPlatformAdded;

    public AssemblingController(PlatformController platformController, TruckController truckController) {
        this.platformController = platformController;
        this.truckController = truckController;
        model = new AssemblingModel();
    }

    public Vector3 HeadPosition => truckController.ReadVehiclePosition();

    public void Init(AssemblingPrototype prototype) {
        model.TruckPrototype = prototype.initTruckPrototype;
        SpawnDriver();
        
        model.PickupPlatformPrototype = prototype.pickupPlatformPrototype;
        SpawnInitPlatforms(prototype.initTruckPrototype.rotation, prototype.initTruckPrototype.position, prototype.initLoadoutPrototypes);
    }

    private void SpawnDriver() {
        truckController.Create(model.TruckPrototype);
    }

    private void SpawnInitPlatforms(Quaternion initRotation, Vector3 initPosition, IEnumerable<LoadoutPrototype> loadoutPrototypes) {
        Vector3 directionStep = initRotation * Vector3.back * 6;
        Vector3 loadoutPosition = initPosition + directionStep; 
        foreach (var loadout in loadoutPrototypes) {    
            AddLoadout(loadoutPosition, loadout, inFrontOrToTheEnd: false);
            loadoutPosition += directionStep;
        }
    }

    public void AddLoadout(Vector3 position, LoadoutPrototype loadoutPrototype, bool inFrontOrToTheEnd) {
        SpawnPlatform(position, out var platformId);
        EquipPlatform(platformId, loadoutPrototype);
        if (inFrontOrToTheEnd) {
            CouplePlatformInFront(platformId);
        } else {
            CouplePlatformToTheEnd(platformId);
        }
        OnPlatformAdded?.Invoke(platformId);
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