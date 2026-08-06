using System;
using System.Collections.Generic;

using UnityEngine;

public class AssemblingController {

    private readonly PlatformController platformController;
    private readonly TruckController truckController;

    private readonly AssemblingModel model;
    private readonly AssemblingView view;

    public event Action<int> OnPlatformAdded;

    public AssemblingController(AssemblingView view, PlatformController platformController, TruckController truckController) {
        this.view = view;
        this.platformController = platformController;
        this.truckController = truckController;
        model = new AssemblingModel();
    }

    public Vector3 HeadPosition => truckController.ReadVehiclePosition();

    public void Init(AssemblingPrototype prototype) {
        view.SetPlatformPreviewPrefab(prototype.platformPreviewPrefab);

        model.TruckPrototype = prototype.initTruckPrototype;
        SpawnDriver();
        
        model.PickupPlatformPrototype = prototype.pickupPlatformPrototype;
        GenerateChainRow(prototype.initTruckPrototype.rotation, prototype.initTruckPrototype.position, prototype.initLoadoutPrototypes);
    }

    public void AddLoadout(Vector3 position, LoadoutPrototype loadout, bool trueInFront_falseToTheEnd) {
        GenerateChainSmoothly(position, loadout, trueInFront_falseToTheEnd);
    }

    public void Update() {
        UpdateSmoothChains();
    }

    private void GenerateChainRow(Quaternion initRotation, Vector3 initPosition, IEnumerable<LoadoutPrototype> loadoutPrototypes) {
        Vector3 directionStep = initRotation * Vector3.back * 6;
        Vector3 loadoutPosition = initPosition + directionStep; 
        foreach (var loadout in loadoutPrototypes) {    
            GenerateChainInstantly(loadoutPosition, loadout, trueInFront_falseToTheEnd: false);
            loadoutPosition += directionStep;
        }
    }

    private void GenerateChainInstantly(Vector3 position, LoadoutPrototype loadout, bool trueInFront_falseToTheEnd) {
        SpawnPlatform(position, out var platformId);
        EquipPlatform(platformId, loadout);
        CouplePlatform(platformId, trueInFront_falseToTheEnd);
        OnPlatformAdded?.Invoke(platformId);
    }

    private Vector3 smoothPosition;
    private LoadoutPrototype? smoothLoadout;
    private int disconnectedPlatformId;
    private float startTime;

    private void GenerateChainSmoothly(Vector3 position, LoadoutPrototype loadout, bool trueInFront_falseToTheEnd) {
        view.ShowPlatformPreview(position);
        var headPlatform = model.ControlledPlatformIds[0];
        platformController.Disconnect(headPlatform);
        disconnectedPlatformId = headPlatform;
        smoothLoadout = loadout;
        smoothPosition = position;
        startTime = Time.time;
    }

    private void UpdateSmoothChains() {
        if (!smoothLoadout.HasValue)
            return;

        var isSafeToConnect = false;
        var headPosition = truckController.ReadVehiclePosition();
        isSafeToConnect = Vector3.Distance(headPosition, smoothPosition) > 4f || Time.time - startTime > 3f;

        if (isSafeToConnect) {
            view.HidePlatformPreview();
            SpawnPlatform(smoothPosition, out var platformId);
            EquipPlatform(platformId, smoothLoadout.Value);

            platformController.Connect(disconnectedPlatformId, platformController.GetVehiclePhysicsId(platformId));
            platformController.Connect(platformId, truckController.ReadVehiclePhysicsId());
            smoothLoadout = null;
        }
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

    private void CouplePlatform(int platformId, bool trueInFront_falseToTheEnd) {
        if (trueInFront_falseToTheEnd) {
            CouplePlatformInFront(platformId);
        } else {
            CouplePlatformToTheEnd(platformId);
        }
    }

    private void CouplePlatformInFront(int platformId) {
        if (model.CoupledPlatformIds.Count > 0) {
            var firstPlatformId = model.CoupledPlatformIds[0];
            platformController.Disconnect(firstPlatformId);

            var newPlatformVehiclePhysicsId = platformController.GetVehiclePhysicsId(platformId);
            platformController.Connect(firstPlatformId, newPlatformVehiclePhysicsId);
        }

        platformController.Connect(platformId, truckController.ReadVehiclePhysicsId());
        model.CoupledPlatformIds.Insert(0, platformId);
    }

    private void CouplePlatformToTheEnd(int platformId) {
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
}