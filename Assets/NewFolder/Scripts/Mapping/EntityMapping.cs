using System;
using System.Collections.Generic;

public class EntityMapping {
    private readonly Dictionary<HitboxId, EntityComponents> mappings = new();
    private readonly List<EntityComponents> resultBuffer = new(32);

    public void CreateMapping(HitboxId hitboxId, EntityComponents components) {
        mappings[hitboxId] = components;
    }

    public void DeleteMapping(HitboxId hitboxId) {
        if (!mappings.Remove(hitboxId))
            throw new ArgumentException($"there is no mapping for hitboxId key {hitboxId}");
    }

    public bool TryFindByHitboxId(HitboxId hitboxId, out EntityComponents components) {
        return mappings.TryGetValue(hitboxId, out components);
    }

    public void FindByHitboxIds(IEnumerable<HitboxId> hitboxIds, out List<EntityComponents> componentsResult) {
        resultBuffer.Clear();
        foreach (var hitboxId in hitboxIds) {
            if (mappings.TryGetValue(hitboxId, out var components))
                resultBuffer.Add(components);
        }
        componentsResult = resultBuffer;
    }
}
