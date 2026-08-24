using System;
using System.Collections.Generic;

public class EntityMapping {
    
    private readonly Dictionary<ProximityId, EntityComponents> proximityMappings = new();
    private readonly Dictionary<RaycastId, EntityComponents> raycastMappings = new();

    private readonly List<EntityComponents> findInfantryIdsBuffer = new (32);

    public void CreateMappings(EntityComponents components) {
        var keyFound = false;
        if (components.raycastId.HasValue) {
            raycastMappings[components.raycastId.Value] = components;
            keyFound = true;
        }
        if (components.proximityId.HasValue) {
            proximityMappings[components.proximityId.Value] = components;
            keyFound = true;
        }
        if (!keyFound) {
            throw new ArgumentException($"components doesn't contains any key ids");
        }
    }

    public void DeleteMappings(ProximityId? proximityId, RaycastId? raycastId) {
        if (proximityId.HasValue) {
            if (!proximityMappings.Remove(proximityId.Value)) {
                throw new ArgumentException($"there is no mappings for proximityId key {proximityId.Value} ");
            }
        }
        if (raycastId.HasValue) {
            if (!raycastMappings.Remove(raycastId.Value)) {
                throw new ArgumentException($"there is no mappings for raycastId key {raycastId.Value} ");
            }
        }
    }

    public bool TryFindByRaycastId(RaycastId raycastId, out EntityComponents components) {
        return raycastMappings.TryGetValue(raycastId, out components);
    }

    public void FindByRaycastIds(List<RaycastId> raycastIds, out List<EntityComponents> infantryIdsResult) {
        findInfantryIdsBuffer.Clear();
        foreach (var nextRaycastId in raycastIds) {
            var components = raycastMappings[nextRaycastId];
            findInfantryIdsBuffer.Add(components);
        }
        infantryIdsResult = findInfantryIdsBuffer;
    }

    public bool TryFindByProximityId(ProximityId proximityId, out EntityComponents components) {
        return proximityMappings.TryGetValue(proximityId, out components);
    }
}