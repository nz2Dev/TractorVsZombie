using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

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
        ProcessChain();
    }

    public void AddLoadout(Vector3 position, LoadoutPrototype loadout, bool trueInFront_falseToTheEnd) {
        AddSegment(position, loadout, trueInFront_falseToTheEnd);
    }

    public void Update() {
        ProcessChain();
    }

    private void GenerateChainRow(Quaternion initRotation, Vector3 initPosition, IEnumerable<LoadoutPrototype> loadoutPrototypes) {
        Vector3 directionStep = initRotation * Vector3.back * 6;
        Vector3 loadoutPosition = initPosition + directionStep; 
        foreach (var loadout in loadoutPrototypes) {    
            AddSegment(loadoutPosition, loadout, trueInFront_falseToTheEnd: false);
            loadoutPosition += directionStep;
        }
    }

    private void AddSegment(Vector3 position, LoadoutPrototype prototype, bool trueInFront_falseToTheEnd) {
        var insertIndex = trueInFront_falseToTheEnd ? 0 : model.Chain.Count;
        model.Chain.Insert(insertIndex, new SegmentState {
            activationPosition = position,
            activationLoadout = prototype
        });
    }

    private void ProcessChain() {
        var headState = new SegmentState { isTruck = true };
        for (int i = 0; i < model.Chain.Count; i++) {
            var state = model.Chain[i];
            ProcessCouple(state, headState);
            headState = state;
        }
    }

    private void ProcessCouple(SegmentState tail, SegmentState head) {
        if (!tail.IsPlatformCreated) {
            Assert.IsFalse(tail.isTruck);

            if (!tail.waitsActivation) {
                view.ShowPlatformPreview(tail.activationPosition);
                tail.waitsActivation = true;
            }

            Vector3? headPosition = null;
            if (head.isTruck) {
                headPosition = truckController.ReadVehiclePosition();
            } else if (head.IsPlatformCreated) {
                headPosition = platformController.ReadPlatformState(head.platformId).position;
            }

            if (headPosition.HasValue && Vector3.Distance(headPosition.Value, tail.activationPosition) > 4) {
                var platformId = platformController.Create(model.PickupPlatformPrototype, tail.activationPosition);
                platformController.SetLoadout(platformId, tail.activationLoadout);
                model.ControlledPlatformIds.Add(platformId);
                tail.waitsActivation = false;
                tail.platformId = platformId;
                view.HidePlatformPreview();
                OnPlatformAdded?.Invoke(platformId);
            }
        }

        if (tail.IsPlatformCreated && !tail.isConnected) {
            int headPhysicsId = -1;
            if (head.isTruck) {
                headPhysicsId = truckController.ReadVehiclePhysicsId();
            } else if (head.IsPlatformCreated) {
                headPhysicsId = platformController.GetVehiclePhysicsId(head.platformId);
            }

            if (headPhysicsId != -1) {
                platformController.Connect(tail.platformId, headPhysicsId);
                tail.isConnected = true;
            }
        }
    }

    private void SpawnDriver() {
        truckController.Create(model.TruckPrototype);
    }

}