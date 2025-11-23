using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly PlayerView view;
    private readonly WeaponController weaponController;
    private readonly VehicleController vehicleController;

    private readonly CombatService combatService;
    private readonly CameraManager cameraManager;
    private readonly SoundManager soundManager;

    private readonly PlayerModel model;

    public PlayerController(PlayerView view, CombatService combatService, CameraManager cameraManager, 
        SoundManager soundManager, WeaponController weaponController, 
        PlayerConfig config, VehicleController vehicleController) {
        this.combatService = combatService;
        this.view = view;
        this.combatService = combatService;
        this.cameraManager = cameraManager;
        this.soundManager = soundManager;

        this.weaponController = weaponController;
        this.vehicleController = vehicleController;
        model = new PlayerModel(config);
    }

    public void Init() {
        cameraManager.InitTopDownFollowTarget(Vector3.zero);
        
        SpawnDriverVehicle(Vector3.zero);

        for (int i = 0; i < model.MaxTrailersCount; i++) {
            SpawnHostVehicle(new Vector3(0, 0, -2f + i * -2f));
        }

        bool flipFlop = false;
        foreach (var trailerHost in model.HostVehicles) {
            flipFlop = !flipFlop;
            var weaponConfig = flipFlop ? model.FirstWeaponConfig : model.SecondWeaponConfig;
            PutWeaponOnHost(trailerHost, weaponConfig);
        }
    }

    public void Update() {
        SyncVehiclePositions();
        DriveHeadVehicle();
        UpdateDriverRamCombat();
        OperateTrailerWeapons();
        UpdateCamera();
    }

    public void SpawnHostWithWeapon(Vector3 position, WeaponConfig weaponConfig) {
        var host = SpawnHostVehicle(position);
        PutWeaponOnHost(host, weaponConfig);
    }

    public Vector3 GetPlayerPosition() {
        return model.DriverPosition;
    }

    private void UpdateCamera() {
        cameraManager.UpdateTopDownFollowPosition(model.DriverPosition);
    }

    private void SyncVehiclePositions() {
        model.DriverPosition = vehicleController.GetVehiclePosition(model.DriverVehicleId);
        combatService.UpdateAgentPosition(model.DriverCombatId, model.DriverPosition);

        foreach (var host in model.HostVehicles) {
            host.Position = vehicleController.GetVehiclePosition(host.VehicleId);
            weaponController.MoveWeapon(host.WeaponId, host.Position);
            combatService.UpdateAgentPosition(host.CombatId, host.Position);
        }
    }

    private void SpawnDriverVehicle(Vector3 driveVehiclePosition) {
        model.DriverVehicleId = vehicleController.SpawnVehicle(driveVehiclePosition, model.DriverVehicleConfig);
        model.DriverCombatId = combatService.RegisterAgent(driveVehiclePosition, alie: true);
    }

    private void DriveHeadVehicle() {
        var steerInput = Input.GetAxis("Horizontal");
        vehicleController.SteerVehicle(model.DriverVehicleId, steerInput);

        var gasInput = Input.GetAxis("Vertical");
        var boost = Input.GetKey(KeyCode.Space);
        vehicleController.DriveVehicle(model.DriverVehicleId, gasInput, boost);
    }

    private void UpdateDriverRamCombat() {
        var affectedCount = combatService.ApplyExplosionDamage(model.DriverCombatId, model.DriverPosition, model.DriverRamRadius, damage: 0);
        for (int i = 0; i < affectedCount; i++) {
            var position = model.DriverPosition + UnityEngine.Random.onUnitSphere * model.DriverRamRadius;
            soundManager.PlayEffectDelayed(position, i * 0.05f, model.DriverRamImpactSound);
        }
    }

    private HostVehicle SpawnHostVehicle(Vector3 position) {
        var combatAgentId = combatService.RegisterAgent(position, alie: true);
        var vehicleId = vehicleController.SpawnVehicle(position, model.TrailerVehicleConfig);
        var hostVehicle = new HostVehicle { Position = position, CombatId = combatAgentId, VehicleId = vehicleId };
        model.HostVehicles.Add(hostVehicle);

        bool isFirstTrailer = model.HostVehicles.Count == 1;
        var headVehicleId = isFirstTrailer ? model.DriverVehicleId : model.HostVehicles[^2].VehicleId;
        vehicleController.ConnectVehicles(headVehicleId, vehicleId);
        return hostVehicle;
    }
    
    private void PutWeaponOnHost(HostVehicle host, WeaponConfig weaponConfig) {
        host.CombatId = combatService.RegisterAgent(host.Position, alie: true);
        host.WeaponId = weaponController.SpawnWeapon(host.CombatId, host.Position, weaponConfig);
        host.WeaponConfig = weaponConfig;
    }

    private void OperateTrailerWeapons() {
        foreach (var model in model.HostVehicles) {
            var enemySearchRadius = model.WeaponConfig.aimConfig.range;
            if (combatService.GetClosestEnemyAgentInRange(model.CombatId, enemySearchRadius, out var agentInfo)) {
                weaponController.AimWeapon(model.WeaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
            }
        }
    }

}