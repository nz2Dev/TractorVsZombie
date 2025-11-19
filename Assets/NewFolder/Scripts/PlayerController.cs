using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly ProjectileController projectileController;
    private readonly RocketController rocketController;

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
    
    private int turelIdCounter;
    private readonly TurelConfig turelConfig;
    private readonly List<Turel> turels = new ();
    private readonly Dictionary<int, int> turelToCombatId = new ();
    private readonly Dictionary<int, TrailerVehicle> turelToVehicle = new ();
    private readonly List<ProjectileState> projectilesStateBuffer = new (64);

    private int rocketLauncherIdCounter;
    private readonly RocketLauncherConfig launcherConfig;
    private readonly List<RocketLauncher> rocketLaunchers = new ();
    private readonly Dictionary<int, int> rocketLauncherToCombatId = new ();
    private readonly Dictionary<int, TrailerVehicle> rocketLauncherToVehicle = new ();

    public PlayerController(VehicleService vehicleService, PlayerView vehicleView, DriverVehicleData driverVehicleData, TrailerVehicleData trailerVehicleData, int trailersCount,
        CombatService combatService, CombatService interactionService, TurelConfig turelConfig,
        RocketLauncherConfig launcherConfig, SoundManager soundManager, CameraManager cameraManager, RewardsMediator rewardsMediator,
        ProjectileController projectileController, RocketController rocketController) {
        this.projectileController = projectileController;
        this.rocketController = rocketController;

        this.combatService = interactionService;
        this.turelConfig = turelConfig;
        this.launcherConfig = launcherConfig;

        this.vehicleService = vehicleService;
        this.playerView = vehicleView;
        this.driverVehicleData = driverVehicleData;
        this.trailerVehicleData = trailerVehicleData;
        this.maxTrailersCount = trailersCount;
        this.combatService = combatService;
        this.soundManager = soundManager;
        this.cameraManager = cameraManager;
        this.rewardsMediator = rewardsMediator;
    }

    public void Init() {
        cameraManager.InitTopDownFollowTarget(Vector3.zero);
        
        SpawnDriverVehicle(Vector3.zero);

        for (int i = 0; i < maxTrailersCount; i++) {
            SpawnTrailerVehicle(new Vector3(0, 0, -2f + i * -2f));
        }

        bool flipFlop = false;
        foreach (var trailerVehicle in trailerVehicles) {
            if (flipFlop = !flipFlop) {
                if (turelConfig != null) {
                    SpawnTurel(trailerVehicle, turelConfig);
                }
            } else {
                if (launcherConfig != null) {
                    SpawnRocketLauncher(trailerVehicle, launcherConfig);
                }
            }
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

        UpdateRocketLauncherOrientation();
        OperateRocketLaunchers();
        UpdateRocketLauncherCombatState();
        UpdateRocketLauncherView();

        UpdateTurelsOrientation();
        OperateTurels();
        UpdateTurelsCombatState();
        UpdateTurelView();
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
                    SpawnTurel(trailerVehicle, reward.configs.turelConfig);
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

    private void SpawnRocketLauncher(TrailerVehicle host, RocketLauncherConfig launcherConfig) {
        var launcherId = rocketLauncherIdCounter++;
        var rocketLauncher = new RocketLauncher(launcherId, host.Position, launcherConfig);
        rocketLaunchers.Add(rocketLauncher);

        rocketLauncherToVehicle[rocketLauncher.Id] = host;

        var rocketLauncherCombatId = combatService.RegisterAgent(rocketLauncher.Position, alie: true);
        rocketLauncherToCombatId[launcherId] = rocketLauncherCombatId;
        
        playerView.AddRocketLauncher(launcherId, rocketLauncher.Position, rocketLauncher.Visuals);
    }

    private void UpdateRocketLauncherOrientation() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var rocketLauncherHost = rocketLauncherToVehicle[rocketLauncher.Id];
            rocketLauncher.Translate(rocketLauncherHost.Position); 
        }
    }

    private void OperateRocketLaunchers() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            if (combatService.GetClosestEnemyAgentInRange(launcherCombatId, rocketLauncher.Radius, out var agentInfo)) {
                rocketLauncher.Aim(agentInfo.position);
            }
            
            if (rocketLauncher.Launch(Time.time, out var trajectory)) {
                rocketController.SpawnRocket(launcherCombatId, trajectory);
            }
        }
    }

    private void UpdateRocketLauncherCombatState() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var rocketLauncherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            combatService.UpdateAgentPosition(rocketLauncherCombatId, rocketLauncher.Position);
        }
    }

    private void UpdateRocketLauncherView() {
        foreach (var rocketLauncher in rocketLaunchers) {
            playerView.UpdateRocketLauncherOrientation(rocketLauncher.Id, rocketLauncher.Position, rocketLauncher.AimPoint, rocketLauncher.RocketAmplitude);
        }
    }

    private void SpawnTurel(TrailerVehicle host, TurelConfig turelConfig) {
        var turelId = turelIdCounter++;
        var turel = new Turel(turelId, host.Position, turelConfig);
        turels.Add(turel);

        turelToVehicle[turel.Id] = host;
        
        var turelCombatId = combatService.RegisterAgent(turel.Position, alie: true);
        turelToCombatId[turel.Id] = turelCombatId;
        
        playerView.AddTurel(turelId, turel.Position, turel.Visuals);
    }

    private void UpdateTurelsOrientation() {
        foreach (var turel in turels) {
            var turelHost = turelToVehicle[turel.Id];
            turel.Move(turelHost.Position);
        }
    }

    private void OperateTurels() {
        foreach (var turel in turels) {    
            var turelCombatId = turelToCombatId[turel.Id];
            
            if (combatService.GetClosestEnemyAgentInRange(turelCombatId, 20, out var closestEnemyAgent)) {
                var aimPoint = closestEnemyAgent.position + 0.5f * closestEnemyAgent.height * Vector3.up;
                turel.Aim(Time.deltaTime, aimPoint);
            }

            if (turel.Fire(Time.time, out var bullet)) {
                projectileController.SpawnBulletProjectile(turelCombatId, bullet, turel.BulletShootAudioClips);
            }
        }
    }

    private void UpdateTurelsCombatState() {
        foreach (var turel in turels) {    
            var turelCombatId = turelToCombatId[turel.Id];
            combatService.UpdateAgentPosition(turelCombatId, turel.Position);
        }
    }

    private void UpdateTurelView() {
        foreach (var turel in turels) {
            playerView.UpdateTurelOrientation(turel.Id, turel.Position, turel.GunForward);
        }
    }

}