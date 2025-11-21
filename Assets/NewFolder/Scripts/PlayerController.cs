using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly WeaponController weaponController;

    private readonly CombatService combatService;
    private readonly VehicleService vehicleService;
    private readonly RewardsMediator rewardsMediator;
    private readonly PlayerView playerView;
    private readonly SoundManager soundManager;
    private readonly CameraManager cameraManager;

    private DriverVehicle driverVehicle;
    private readonly DriverVehicleData driverVehicleData;
    private int driverVehicleCombatId;
    private int driverVehiclePhysicsId;
    private int driverVehicleEngineSoundId;
    private int driverVehicleViewId;
    
    private readonly int maxTrailersCount;
    private readonly TrailerVehicleData trailerVehicleData;
    private readonly List<TrailerVehicle> trailerVehicles = new ();
    private readonly List<int> trailerPhysicIds = new ();
    private readonly List<int> trailerViewIds = new ();

    private readonly List<int> weaponIds = new ();
    private readonly List<int> weaponCombatIds = new ();
    private readonly List<TrailerVehicle> weaponVehicleRefs = new ();
    
    private readonly WeaponConfig firstWeaponConfig;
    private readonly WeaponConfig secondWeaponConfig;

    public PlayerController(VehicleService vehicleService, PlayerView vehicleView, DriverVehicleData driverVehicleData, TrailerVehicleData trailerVehicleData, int trailersCount,
        CombatService combatService, CombatService interactionService, SoundManager soundManager, CameraManager cameraManager, RewardsMediator rewardsMediator, 
        WeaponController weaponController, WeaponConfig firstWeaponConfig, WeaponConfig secondWeaponConfig) {
        this.combatService = interactionService;
        this.vehicleService = vehicleService;
        this.playerView = vehicleView;
        this.driverVehicleData = driverVehicleData;
        this.trailerVehicleData = trailerVehicleData;
        this.maxTrailersCount = trailersCount;
        this.combatService = combatService;
        this.soundManager = soundManager;
        this.cameraManager = cameraManager;
        this.rewardsMediator = rewardsMediator;

        this.weaponController = weaponController;
        this.firstWeaponConfig = firstWeaponConfig;
        this.secondWeaponConfig = secondWeaponConfig;
    }

    public void Init() {
        cameraManager.InitTopDownFollowTarget(Vector3.zero);
        
        SpawnDriverVehicle(Vector3.zero);

        for (int i = 0; i < maxTrailersCount; i++) {
            SpawnTrailerVehicle(new Vector3(0, 0, -2f + i * -2f));
        }

        bool flipFlop = false;
        foreach (var trailerVehicle in trailerVehicles) {
            flipFlop = !flipFlop;
            var weaponConfig = flipFlop ? firstWeaponConfig : secondWeaponConfig;
            AttackWeaponOnTrailer(trailerVehicle, weaponConfig);
        }
    }

    public void Update() {
        ReadVehiclesOrientation();
        ReadDriveVehicleInput();
        UpdateDriveVehiclePhysics();
        UpdateDriveVehicleSounds();
        UpdateVehiclesView();
        UpdateDriveVehicleCombat();

        DiscoverRewards();
        CollectRewards();
        FilterRemovedRewards();
        ClearRewardsEvents();

        UpdateCamera();
        SyncWeaponsWithTrailers();
    }

    private void DiscoverRewards() {
        foreach (var rewardState in rewardsMediator.RewardAddedEvents) {
            playerView.SpawnReward(rewardState.id, rewardState.position, rewardState.rewardVisuals);
        }
    }

    private List<RewardState> rewardsBuffer = new (10);

    private void CollectRewards() {
        if (rewardsMediator.CollectRewards(driverVehicle.Position, driverVehicle.RewardCollectRadius, rewardsBuffer)) {
            foreach (var reward in rewardsBuffer) {
                if (reward.rewardType == RewardType.TurelWeapon) {
                    var trailerVehicle = SpawnTrailerVehicle(reward.position);
                    AttackWeaponOnTrailer(trailerVehicle, firstWeaponConfig);
                }
            }
        }
    }

    private void FilterRemovedRewards() {
        foreach (var rewardState in rewardsMediator.RewardRemovedEvents) {
            playerView.DespawnReward(rewardState.id);
        }
    }

    private void ClearRewardsEvents() {
        rewardsMediator.ClearEvents();
    }

    private void UpdateCamera() {
        cameraManager.UpdateTopDownFollowPosition(driverVehicle.Position);
    }

    private void SpawnDriverVehicle(Vector3 driveVehiclePosition) {
        driverVehicle = new DriverVehicle(driverVehicleData);
        driverVehiclePhysicsId = vehicleService.CreateVehicle(driveVehiclePosition, driverVehicleData.physicsPrefab);
        driverVehicleCombatId = combatService.RegisterAgent(driveVehiclePosition, alie: true);
        driverVehicleEngineSoundId = soundManager.StartLoop(driverVehicle.Position, driverVehicle.EngineIdleSound);
        driverVehicleViewId = playerView.AddVehicle(driveVehiclePosition, driverVehicle.VisualsPrefab);
    }

    private void ReadDriveVehicleInput() {
        var steerInput = Input.GetAxis("Horizontal");
        driverVehicle.Steer(steerInput);

        var gasInput = Input.GetAxis("Vertical");
        var boost = Input.GetKey(KeyCode.Space);
        driverVehicle.Throttle(gasInput, Time.deltaTime, boost);
    }

    private void UpdateDriveVehiclePhysics() {
        vehicleService.SetVehicleEngineTorque(driverVehiclePhysicsId, driverVehicle.MotorTorque);
        vehicleService.SetVehicleSteer(driverVehiclePhysicsId, driverVehicle.SteerDegrees);
    }

    private void UpdateDriveVehicleSounds() {
        var enginePitch = 0.5f + driverVehicle.DrivePower;
        var engineVolume = 0.5f + driverVehicle.DrivePower;
        soundManager.UpdateLoop(driverVehicleEngineSoundId, driverVehicle.Position, enginePitch, engineVolume);
    }

    private void UpdateDriveVehicleCombat() {
        combatService.UpdateAgentPosition(driverVehicleCombatId, driverVehicle.Position);
        var affectedCount = combatService.ApplyExplosionDamage(driverVehicleCombatId, driverVehicle.Position, radius: driverVehicle.RamRadius, damage: 0);
        for (int i = 0; i < affectedCount; i++) {
            var position = driverVehicle.Position + UnityEngine.Random.onUnitSphere * driverVehicle.RamRadius;
            soundManager.PlayEffectDelayed(position, i * 0.05f, driverVehicle.HitImpactSounds);
        }
    }

    private TrailerVehicle SpawnTrailerVehicle(Vector3 position) {
        var trailerVehicle = new TrailerVehicle(trailerVehicleData);
        trailerVehicles.Add(trailerVehicle);

        var trailerPhysicsId = vehicleService.CreateVehicle(position, trailerVehicle.PhysicsPrefab);
        trailerPhysicIds.Add(trailerPhysicsId);

        bool isFirstTrailer = trailerVehicles.Count == 1;
        var headPhysicsId = isFirstTrailer ? driverVehiclePhysicsId : trailerPhysicIds[^2];
        vehicleService.MakeTowingConnection(headPhysicsId, trailerPhysicsId);
        
        var trailerViewId = playerView.AddVehicle(position, trailerVehicle.VisualsData);
        trailerViewIds.Add(trailerViewId);
        
        return trailerVehicle;
    }

    private void ReadVehiclesOrientation() {
        var driverVehiclePose = vehicleService.GetVehicleState(driverVehiclePhysicsId);
        driverVehicle.UpdatePhysicsState(driverVehiclePose);
        
        for (int trailerIndex = 0; trailerIndex < trailerVehicles.Count; trailerIndex++) {
            var trailerVehicle = trailerVehicles[trailerIndex];
            var trailerPhysicsId = trailerPhysicIds[trailerIndex];
            var trailerPose = vehicleService.GetVehicleState(trailerPhysicsId);
            trailerVehicle.UpdatePhysicsState(trailerPose);   
        }
    }

    private void UpdateVehiclesView() {
        playerView.UpdateVehiclePose(driverVehicleViewId, driverVehicle.PhysicsState);
        
        for (int trailerIndex = 0; trailerIndex < trailerVehicles.Count; trailerIndex++) {
            var vehicle = trailerVehicles[trailerIndex];
            var vehicleViewIndex = trailerViewIds[trailerIndex];
            
            playerView.UpdateVehiclePose(vehicleViewIndex, vehicle.PhysicsState);   
        }
    }

    private void AttackWeaponOnTrailer(TrailerVehicle host, WeaponConfig weaponConfig) {
        var weaponCombatId = combatService.RegisterAgent(host.Position, alie: true);
        var weaponId = weaponController.SpawnWeapon(weaponCombatId, host.Position, weaponConfig);
        weaponIds.Add(weaponId);
        weaponCombatIds.Add(weaponCombatId);
        weaponVehicleRefs.Add(host);
    }

    private void SyncWeaponsWithTrailers() {
        for (int i = 0; i < weaponIds.Count; i++) {
            var weaponId = weaponIds[i];
            var weaponCombatId = weaponCombatIds[i];
            var weaponHost = weaponVehicleRefs[i];
            weaponController.MoveWeapon(weaponId, weaponHost.Position);
            combatService.UpdateAgentPosition(weaponCombatId, weaponHost.Position);
        }        
    }

}