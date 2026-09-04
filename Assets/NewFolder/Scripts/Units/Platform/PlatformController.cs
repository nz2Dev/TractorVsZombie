using System.Collections.Generic;

using Combat;

using UnityEngine;

public class PlatformController {

    private readonly CombatSystem combatSystem;
    private readonly LoadoutController loadoutController;
    private readonly RamEffectController ramEffect;
    private readonly VehicleService vehicleService;
    private readonly ProximityService proximityService;
    private readonly RaycastService raycastService;
    private readonly EntityMapping entityMapping;
    private readonly PlatformView view;

    private int idCounter;
    private readonly Dictionary<int, PlatformModel> registry = new();

    public PlatformController(CombatSystem combatSystem, LoadoutController loadoutController, RamEffectController ramEffect, VehicleService vehicleService, PlatformView view, ProximityService proximityService, RaycastService raycastService, EntityMapping entityMapping) {
        this.combatSystem = combatSystem;
        this.loadoutController = loadoutController;
        this.ramEffect = ramEffect;
        this.vehicleService = vehicleService;
        this.view = view;
        this.proximityService = proximityService;
        this.raycastService = raycastService;
        this.entityMapping = entityMapping;
    }

    public void Update() {
        SyncPositions();
    }

    public virtual int Create(PlatformPrototype prototype, Vector3 position = default) {
        var nextId = ++idCounter;
        var initPosition = position == default ? prototype.position : position;
        var model = new PlatformModel(nextId, initPosition, prototype.config, prototype.loadoutOffset);
        registry[model.Id] = model;

        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.VehiclePhysicsId = vehicleService.CreateVehicle(model.Position, prototype.vehiclePrefab);
        model.ProximityId = proximityService.AddPoint(initPosition, CombatSystem.GetProximityLayerForFaction(prototype.combatPrototype.alie));
        model.RaycastId = raycastService.RegisterMarker(initPosition, prototype.raycastMarkerPrefab, CombatSystem.GetRaycastLayerForFaction(prototype.combatPrototype.alie));

        model.RamId = ramEffect.StartNew(model.CombatId, model.VehiclePhysicsId, prototype.combatPrototype.alie, prototype.ramPrototype);

        entityMapping.CreateMappings(new EntityComponents {
            proximityId = model.ProximityId,
            raycastId = model.RaycastId,
            combatId = model.CombatId
        });
        
        view.AddPlatform(model.Id, model.Position, prototype.visualsPrefab);
        return model.Id;
    }

    public virtual void Connect(int tailPlatformId, int headVehiclePhysicsId) {
        var tailPlatform = registry[tailPlatformId];
        var headState = vehicleService.GetVehicleState(headVehiclePhysicsId);

        var towardHeadRotation = Quaternion.LookRotation((headState.position - tailPlatform.Position).normalized, Vector3.up);
        vehicleService.UpdateVehiclePose(tailPlatform.VehiclePhysicsId, tailPlatform.Position, towardHeadRotation);
        vehicleService.MakeTowingConnection(headVehiclePhysicsId, tailPlatform.VehiclePhysicsId);
    }

    public virtual void Disconnect(int platformId) {
        var platform = registry[platformId];
        vehicleService.ClearTowingConnection(platform.VehiclePhysicsId);
    }

    public virtual void SetLoadout(int platformId, LoadoutPrototype loadoutPrototype) {
        var platform = registry[platformId];

        if (platform.LoadoutId != 0) {
            loadoutController.DeleteLoadout(platform.LoadoutId);
        }

        loadoutPrototype.position = platform.Position + platform.LoadoutOffset;
        platform.LoadoutId = loadoutController.SpawnLoadout(platform.CombatId, loadoutPrototype);
    }

    public virtual int GetVehiclePhysicsId(int platformId) {
        return registry[platformId].VehiclePhysicsId;
    }

    public void ReadAllPlatforms(IList<PlatformState> statesBuffer) {
        statesBuffer.Clear();
        foreach (var platform in registry.Values) {
            statesBuffer.Add(ReadPlatformState(platform));
        }
    }

    public virtual PlatformState ReadPlatformState(int platformId) {
        return ReadPlatformState(registry[platformId]);
    }

    public virtual PlatformState ReadPlatformState(PlatformModel platform) {
        var loadoutState = default (LoadoutState);
        if (platform.LoadoutId != 0) {
            loadoutState = loadoutController.ReadLoadoutState(platform.LoadoutId);
        }
        return new PlatformState (
            position: platform.Position,
            combatId: platform.CombatId,
            combatState: combatSystem.ReadState(platform.CombatId),
            vehiclePhysicsId: platform.VehiclePhysicsId,
            weaponId: loadoutState.weaponId,
            weaponState: loadoutState.weaponState,
            platformId: platform.Id
        );
    }

    private void SyncPositions() {
        foreach (var host in registry.Values) {
            host.VehiclePhysicsState = vehicleService.GetVehicleState(host.VehiclePhysicsId);
            host.Position = host.VehiclePhysicsState.position;
            view.UpdatePlatformPose(host.Id, host.VehiclePhysicsState);

            if (host.LoadoutId != 0) {
                loadoutController.MoveLoadout(host.LoadoutId, host.Position + host.LoadoutOffset, host.VehiclePhysicsState.rotation);
            }

            proximityService.UpdatePoint(host.ProximityId, host.Position);
            raycastService.UpdateMarker(host.RaycastId, host.Position);
            ramEffect.Forward(host.RamId, host.Position);
        }
    }

}
