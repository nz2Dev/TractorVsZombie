using System;
using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly CombatService combatService;
    private readonly VehicleService vehicleService;
    private readonly ProjectileService projectileService;
    private readonly RewardsMediator rewardsMediator;
    private readonly VehicleView vehicleView;
    private readonly WeaponView view;
    private readonly RewardsView rewardsView;
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
    private readonly Dictionary<int, int> turelToProjectileGroupId = new ();
    private readonly List<ProjectileState> projectilesStateBuffer = new (64);

    private int rocketLauncherIdCounter;
    private readonly RocketLauncherConfig launcherConfig;
    private readonly List<RocketLauncher> rocketLaunchers = new ();
    private readonly Dictionary<int, int> rocketLauncherToCombatId = new ();
    private readonly Dictionary<int, TrailerVehicle> rocketLauncherToVehicle = new ();

    private int rocketIdCounter;
    private readonly Dictionary<int, List<Rocket>> rocketLauncherRocketsRegistry = new ();

    public PlayerController(VehicleService vehicleService, VehicleView vehicleView, DriverVehicleData driverVehicleData, TrailerVehicleData trailerVehicleData, int trailersCount,
        CombatService combatService, WeaponView weaponView, CombatService interactionService, TurelConfig turelConfig, ProjectileService projectileService,
        RocketLauncherConfig launcherConfig, SoundManager soundManager, CameraManager cameraManager, RewardsView rewardsView, RewardsMediator rewardsMediator) {
        this.view = weaponView;
        this.combatService = interactionService;
        this.turelConfig = turelConfig;
        this.projectileService = projectileService;
        this.launcherConfig = launcherConfig;

        this.vehicleService = vehicleService;
        this.vehicleView = vehicleView;
        this.driverVehicleData = driverVehicleData;
        this.trailerVehicleData = trailerVehicleData;
        this.maxTrailersCount = trailersCount;
        this.combatService = combatService;
        this.soundManager = soundManager;
        this.cameraManager = cameraManager;
        this.rewardsView = rewardsView;
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
        UpdateVehiclePhysics();

        DiscoverRewards();
        CollectRewards();
        FilterRemovedRewards();
        ClearRewardsEvents();

        UpdateCamera();

        UpdateRocketLandingCombat();
        FilterElapsedRockets();
        UpdateRocketLauncherOrientation();
        OperateRocketLaunchers();
        UpdateRocketLauncherCombatState();
        UpdateRocketLauncherView();

        UpdateProjectileHits();
        FilterDeadProjectiles();
        UpdateTurelsOrientation();
        OperateTurels();
        UpdateTurelsCombatState();
        UpdateTurelView();
    }

    private void DiscoverRewards() {
        foreach (var rewardState in rewardsMediator.RewardAddedEvents) {
            rewardsView.SpawnReward(rewardState.id, rewardState.position, rewardState.rewardVisuals);
        }
    }

    private List<RewardState> rewardsBuffer = new (10);

    private void CollectRewards() {
        if (rewardsMediator.CollectRewards(driverVehicle.BodyPose.position, driverVehicle.RewardCollectRadius, rewardsBuffer)) {
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
            rewardsView.DespawnReward(rewardState.id);
        }
    }

    private void ClearRewardsEvents() {
        rewardsMediator.ClearEvents();
    }

    private void UpdateVehiclePhysics() {
        var driverPhysicsData = driverVehicle.PhysicsData;
        vehicleService.UpdateBase(driverVehiclePhysicsId, driverVehicle.PhysicsData.mass);
        vehicleService.UpdateWheels(driverVehiclePhysicsId, driverPhysicsData.wheelMass, driverPhysicsData.forwardFrictionStiffness, driverPhysicsData.sidewayFrictionStiffness);

        for (int i = 0; i < trailerVehicles.Count; i++) {
            var trailerVehicle = trailerVehicles[i];
            var trailerPhysicsId = trailerPhysicIds[i];
            var physicsData = trailerVehicle.PhysicsData;
            vehicleService.UpdateBase(trailerPhysicsId, physicsData.mass);
            vehicleService.UpdateWheels(trailerPhysicsId, physicsData.wheelMass, physicsData.forwardFrictionStiffness, physicsData.sidewayFrictionStiffness);
        }
    }

    private void UpdateCamera() {
        cameraManager.UpdateTopDownFollowPosition(driverVehicle.BodyPose.position);
    }

    private void SpawnDriverVehicle(Vector3 driveVehiclePosition) {
        driverVehicle = new DriverVehicle(driverVehicleData);
        driverVehiclePhysicsId = vehicleService.CreateVehicle(driveVehiclePosition, driverVehicleData.physicsData);
        driverVehicleCombatId = combatService.RegisterAgent(driveVehiclePosition, alie: true);
        driverVehicleEngineSoundId = soundManager.StartLoop(driverVehicle.BodyPose.position, driverVehicle.EngineIdleSound);
        driverVehicleViewId = vehicleView.AddDriverVehicle(driveVehiclePosition, driverVehicle.PhysicsData, driverVehicle.VisualsData);
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
        soundManager.UpdateLoop(driverVehicleEngineSoundId, driverVehicle.BodyPose.position, enginePitch, engineVolume);
    }

    private void UpdateDriveVehicleCombat() {
        combatService.UpdateAgentPosition(driverVehicleCombatId, driverVehicle.BodyPose.position);
        var affectedCount = combatService.ApplyExplosionDamage(driverVehicleCombatId, driverVehicle.BodyPose.position, radius: driverVehicle.RamRadius, damage: 0);
        for (int i = 0; i < affectedCount; i++) {
            var position = driverVehicle.BodyPose.position + UnityEngine.Random.onUnitSphere * driverVehicle.RamRadius;
            soundManager.PlayEffectDelayed(position, i * 0.05f, driverVehicle.HitImpactSounds);
        }
    }

    private TrailerVehicle SpawnTrailerVehicle(Vector3 position) {
        var trailerVehicle = new TrailerVehicle(trailerVehicleData);
        trailerVehicles.Add(trailerVehicle);

        var trailerPhysicsId = vehicleService.CreateVehicle(position, trailerVehicle.PhysicsData);
        trailerPhysicIds.Add(trailerPhysicsId);

        bool isFirstTrailer = trailerVehicles.Count == 1;
        var headPhysicsId = isFirstTrailer ? driverVehiclePhysicsId : trailerPhysicIds[^2];
        vehicleService.MakeTowingConnection(headPhysicsId, trailerPhysicsId);
        
        var trailerViewId = vehicleView.AddTrailerVehicle(position, trailerVehicle.PhysicsData, trailerVehicle.VisualsData);
        trailerViewIds.Add(trailerViewId);
        
        return trailerVehicle;
    }

    private void ReadVehiclesOrientation() {
        var driverVehiclePose = vehicleService.GetVehiclePose(driverVehiclePhysicsId);
        driverVehicle.OrientBody(driverVehiclePose);
        for (int wheelAxisIndex = 0; wheelAxisIndex < driverVehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
            var wheelAxisPose = vehicleService.GetVehicleWheelAxisPose(driverVehiclePhysicsId, wheelAxisIndex);
            driverVehicle.OrientWheelAxis(wheelAxisIndex, wheelAxisPose);
        }

        for (int trailerIndex = 0; trailerIndex < trailerVehicles.Count; trailerIndex++) {
            var trailerVehicle = trailerVehicles[trailerIndex];
            var trailerPhysicsId = trailerPhysicIds[trailerIndex];
            
            var vehiclePose = vehicleService.GetVehiclePose(trailerPhysicsId);
            trailerVehicle.OrientBody(vehiclePose);

            var towingTonguePose = vehicleService.GetTowingTonguePose(trailerPhysicsId);
            trailerVehicle.OrientTowingTonque(towingTonguePose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < trailerVehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                var wheelAxisPose = vehicleService.GetVehicleWheelAxisPose(trailerPhysicsId, wheelAxisIndex);
                trailerVehicle.OrientWheelAxis(wheelAxisIndex, wheelAxisPose);
            }   
        }
    }

    private void UpdateVehiclesView() {
        vehicleView.UpdateVehiclePose(driverVehicleViewId, driverVehicle.BodyPose);
        for (int wheelAxisIndex = 0; wheelAxisIndex < driverVehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
            vehicleView.UpdateWheelAxisPose(driverVehicleViewId, wheelAxisIndex, driverVehicle.WheelAxisPoses[wheelAxisIndex]);
        }

        for (int trailerIndex = 0; trailerIndex < trailerVehicles.Count; trailerIndex++) {
            var vehicle = trailerVehicles[trailerIndex];
            var vehicleViewIndex = trailerViewIds[trailerIndex];
            
            vehicleView.UpdateVehiclePose(vehicleViewIndex, vehicle.BodyPose);
            vehicleView.UpdateTowingTonguePose(vehicleViewIndex, vehicle.TowingTonqueRotation);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                vehicleView.UpdateWheelAxisPose(vehicleViewIndex, wheelAxisIndex, vehicle.WheelAxisPoses[wheelAxisIndex]);
            }   
        }
    }

    private void SpawnRocketLauncher(TrailerVehicle host, RocketLauncherConfig launcherConfig) {
        var launcherId = rocketLauncherIdCounter++;
        var rocketLauncher = new RocketLauncher(launcherId, host.BodyPose.position, launcherConfig);
        rocketLaunchers.Add(rocketLauncher);

        rocketLauncherToVehicle[rocketLauncher.Id] = host;

        var rocketLauncherCombatId = combatService.RegisterAgent(rocketLauncher.Position, alie: true);
        rocketLauncherToCombatId[launcherId] = rocketLauncherCombatId;

        rocketLauncherRocketsRegistry[launcherId] = new List<Rocket>();
        
        view.AddRocketLauncher(launcherId, rocketLauncher.Position, rocketLauncher.Visuals);
    }

    private void UpdateRocketLauncherOrientation() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var rocketLauncherHost = rocketLauncherToVehicle[rocketLauncher.Id];
            rocketLauncher.Translate(rocketLauncherHost.BodyPose.position); 
        }
    }

    private void OperateRocketLaunchers() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            if (combatService.GetClosestEnemyAgentInRange(launcherCombatId, rocketLauncher.Radius, out var agentInfo)) {
                rocketLauncher.Aim(agentInfo.position);
            }
            
            if (rocketLauncher.Launch(Time.time, out var trajectory)) {
                SpawnRocket(rocketLauncher, trajectory);
            }
        }
    }

    private void UpdateRocketLauncherCombatState() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var rocketLauncherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            combatService.UpdateAgentPosition(rocketLauncherCombatId, rocketLauncher.Position);
        }
    }

    private void SpawnRocket(RocketLauncher rocketLauncher, RocketTrajectory trajectory) {
        var nextRocketId = rocketIdCounter++;
        var rocket = new Rocket(nextRocketId, trajectory, Time.time, rocketLauncher.RocketFlyDuration);
        rocketLauncherRocketsRegistry[rocketLauncher.Id].Add(rocket);
        view.ShowRocketFly(rocketLauncher.Id, nextRocketId, trajectory, rocketLauncher.RocketFlyDuration);
        soundManager.PlayEffect(trajectory.launchPoint, rocketLauncher.RocketLaunchEffects);
    }

    private void UpdateRocketLandingCombat() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            
            foreach (var rocket in rocketLauncherRocketsRegistry[rocketLauncher.Id]) {
                if (rocket.ForwardLandingTime(Time.time)) {
                    combatService.ApplyExplosionDamage(launcherCombatId, rocket.Trajectory.landPoint, 3, rocketLauncher.RocketDamage);
                    var center = rocket.Trajectory.landPoint;
                    var color = Color.red;
                    var duration = 1f;
                    var radius = 3;
                    Debug.DrawLine(center, center + Vector3.right * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.left * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.forward * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.back * radius, color, duration);
                    view.ShowRocketExplosion(rocketLauncher.Id, rocket.Id);
                    soundManager.PlayEffect(rocket.Trajectory.landPoint, rocketLauncher.ExplodeEffectClips);
                }
            }   
        }
    }

    private void FilterElapsedRockets() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherRockets = rocketLauncherRocketsRegistry[rocketLauncher.Id];
            for (int i = 0; i < launcherRockets.Count; i++) {
                var rocket = launcherRockets[i];
                if (rocket.Landed) {
                    launcherRockets.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private void UpdateRocketLauncherView() {
        foreach (var rocketLauncher in rocketLaunchers) {
            view.UpdateRocketLauncherOrientation(rocketLauncher.Id, rocketLauncher.Position, rocketLauncher.AimPoint, rocketLauncher.RocketAmplitude);
        }
    }

    private void SpawnTurel(TrailerVehicle host, TurelConfig turelConfig) {
        var turelId = turelIdCounter++;
        var turel = new Turel(turelId, host.BodyPose.position, turelConfig);
        turels.Add(turel);

        turelToVehicle[turel.Id] = host;
        
        var turelCombatId = combatService.RegisterAgent(turel.Position, alie: true);
        turelToCombatId[turel.Id] = turelCombatId;

        var turelProjectilesGroupId = projectileService.AddGroup();
        turelToProjectileGroupId[turel.Id] = turelProjectilesGroupId;
        
        view.AddTurel(turelId, turel.Position, turel.Visuals);
    }

    private void UpdateTurelsOrientation() {
        foreach (var turel in turels) {
            var turelHost = turelToVehicle[turel.Id];
            turel.Move(turelHost.BodyPose.position);
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
                SpawnBulletProjectile(turel, bullet);
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
            view.UpdateTurelOrientation(turel.Id, turel.Position, turel.GunForward);
        }
    }

    private void SpawnBulletProjectile(Turel turel, Bullet bullet) {
        var projectileGroupId = turelToProjectileGroupId[turel.Id];
        var projectileId = projectileService.CreateProjectile(projectileGroupId, bullet.firePoint, bullet.velocity, 5f);
        view.ShowBulletShoot(turel.Id, projectileId, bullet.velocity);
        soundManager.PlayEffect(bullet.firePoint, turel.BulletShootAudioClips);
    }

    private void FilterDeadProjectiles() {
        foreach (var turel in turels) {
            var projectileGroup = turelToProjectileGroupId[turel.Id];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);
            
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged) {
                    view.ShowBulletDisappear(turel.Id, projectileState.id);
                }
            }
        }
    }

    private void UpdateProjectileHits() {   
        foreach (var turel in turels) {
            var projectileGroup = turelToProjectileGroupId[turel.Id];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);

            var combatId = turelToCombatId[turel.Id];
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged)
                    continue;

                if (combatService.ApplyProjectileDamage(combatId, projectileState.position, projectileState.velocity, turel.BulletDamage)) {
                    projectileService.KillProjectile(projectileState.id);
                    view.ShowBulletCrash(turel.Id, projectileState.id);
                }
            }
        }
    }
}