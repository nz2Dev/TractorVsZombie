using System.Collections.Generic;

using UnityEngine;

public sealed class HitboxSystem {
    private readonly FactionRegistry factionRegistry;
    private readonly PoseRegistry poseRegistry;
    private readonly RaycastService raycastService;
    private readonly ProximityService proximityService;
    private readonly Dictionary<HitboxId, HitboxModel> models = new();
    private readonly Dictionary<RaycastId, HitboxId> raycastToHitbox = new();
    private readonly Dictionary<ProximityId, HitboxId> proximityToHitbox = new();
    private readonly List<HitboxId> hitboxResultBuffer = new();
    private int idCounter;

    public HitboxSystem(FactionRegistry factionRegistry, PoseRegistry poseRegistry, RaycastService raycastService, ProximityService proximityService) {
        this.factionRegistry = factionRegistry;
        this.poseRegistry = poseRegistry;
        this.raycastService = raycastService;
        this.proximityService = proximityService;
    }

    public HitboxId Add(FactionId factionId, PoseId poseId, RaycastMarker markerPrefab) {
        var position = poseRegistry.Read(poseId).position;
        return Add(factionId, poseId, position, markerPrefab);
    }

    public HitboxId Add(FactionId factionId, Vector3 position, RaycastMarker markerPrefab) {
        return Add(factionId, null, position, markerPrefab);
    }

    private HitboxId Add(FactionId factionId, PoseId? poseId, Vector3 position, RaycastMarker markerPrefab) {
        var id = new HitboxId(++idCounter);
        var factionState = factionRegistry.ReadState(factionId);
        var raycastLayer = FactionRegistry.GetRaycastLayerForFaction(factionState.alie);
        var proximityLayer = FactionRegistry.GetProximityLayerForFaction(factionState.alie);
        var proximityId = proximityService.AddPoint(position, proximityLayer);
        var raycastId = raycastService.RegisterMarker(position, markerPrefab, raycastLayer);
        var model = new HitboxModel(id, factionState, raycastId, proximityId, poseId, position);

        models.Add(id, model);
        raycastToHitbox.Add(raycastId, id);
        proximityToHitbox.Add(proximityId, id);
        return id;
    }

    public void Remove(HitboxId id) {
        if (!models.Remove(id, out var model))
            return;

        raycastService.UnregisterMarker(model.RaycastId);
        raycastToHitbox.Remove(model.RaycastId);
        proximityService.RemovePoint(model.ProximityId);
        proximityToHitbox.Remove(model.ProximityId);
    }

    public void UpdatePosition(HitboxId id, Vector3 position) {
        var model = models[id];
        model.Position = position;
        raycastService.UpdateMarker(model.RaycastId, position);
        proximityService.UpdatePoint(model.ProximityId, position);
    }

    public void Update() {
        foreach (var model in models.Values) {
            if (!model.PoseId.HasValue)
                continue;

            var position = poseRegistry.Read(model.PoseId.Value).position;
            if (position == model.Position)
                continue;

            model.Position = position;
            raycastService.UpdateMarker(model.RaycastId, position);
            proximityService.UpdatePoint(model.ProximityId, position);
        }
    }

    public int Overlap(Vector3 position, float radius, bool factionIsAlly, out List<HitboxId> idsResult) {
        var layer = FactionRegistry.GetRaycastLayerForFaction(factionIsAlly);
        raycastService.Overlap(position, radius, layer, out var raycastIds);
        hitboxResultBuffer.Clear();
        foreach (var raycastId in raycastIds) {
            if (raycastToHitbox.TryGetValue(raycastId, out var hitboxId))
                hitboxResultBuffer.Add(hitboxId);
        }
        idsResult = hitboxResultBuffer;
        return idsResult.Count;
    }

    public bool Raycast(Ray ray, float maxDistance, bool factionIsAlly, out HitboxId hitboxId, out RaycastHit hitInfo) {
        var layer = FactionRegistry.GetRaycastLayerForFaction(factionIsAlly);
        if (raycastService.Raycast(ray, maxDistance, layer, out var raycastId, out hitInfo)
            && raycastToHitbox.TryGetValue(raycastId, out hitboxId))
            return true;

        hitboxId = default;
        return false;
    }

    public bool QueryNearest(Vector3 position, bool factionIsAlly, out HitboxId hitboxId) {
        var layer = FactionRegistry.GetProximityLayerForFaction(factionIsAlly);
        if (proximityService.QueryNearestPoint(position, layer, out var proximityId)
            && proximityToHitbox.TryGetValue(proximityId, out hitboxId))
            return true;

        hitboxId = default;
        return false;
    }

    public Vector3 GetPosition(HitboxId id) {
        return proximityService.GetPoint(models[id].ProximityId);
    }

    public RaycastState ReadState(HitboxId id) {
        return raycastService.ReadState(models[id].RaycastId);
    }
}
