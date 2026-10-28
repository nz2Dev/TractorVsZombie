using System.Collections.Generic;

public sealed class DropSystem {

    private readonly HealthSystem healthSystem;
    private readonly PoseRegistry poseRegistry;
    private readonly RewardController rewardController;
    private readonly Dictionary<DropId, DropModel> models = new();
    private int idCounter;

    public DropSystem(HealthSystem healthSystem, PoseRegistry poseRegistry, RewardController rewardController) {
        this.healthSystem = healthSystem;
        this.poseRegistry = poseRegistry;
        this.rewardController = rewardController;
    }

    public DropId Add(DropPrototype prototype, HealthId hostHealthId, PoseId hostPoseId) {
        var id = new DropId(++idCounter);
        models.Add(id, new DropModel(id, prototype.rewardPrototype, prototype.inheritHostRotation, hostHealthId, hostPoseId));
        return id;
    }

    public void Remove(DropId id) {
        models.Remove(id);
    }

    public void Update() {
        foreach (var model in models.Values) {
            if (model.HasDropped)
                continue;

            var healthState = healthSystem.ReadState(model.HostHealthId);
            if (!healthState.isDead)
                continue;

            var pose = poseRegistry.Read(model.HostPoseId);
            var rotation = model.InheritHostRotation ? pose.rotation : default;
            rewardController.Create(model.RewardPrototype, pose.position, rotation);
            model.HasDropped = true;
        }
    }
}
