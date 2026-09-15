using System.Collections.Generic;

using UnityEngine;

public enum ReservedLayerCode {
    First,
    Second
}

public class RaycastService {
    
    private readonly Dictionary<RaycastId, RaycastMarker> markersRegistry = new();
    private readonly Dictionary<GameObject, RaycastId> markerToId = new();

    private readonly Collider[] overlapBuffer;
    private readonly List<RaycastId> idsResultBuffer;
    private readonly RaycastServiceConfig config;

    private int idCounter;

    public RaycastService(RaycastServiceConfig config) {
        this.config = config;
        overlapBuffer = new Collider[config.overlapBufferSize];
        idsResultBuffer = new (config.overlapBufferSize);
    }

    public RaycastId RegisterMarker(Vector3 position, RaycastMarker markerPrefab, ReservedLayerCode layerCode) {
        var nextId = new RaycastId(++idCounter);
        var marker = GameObject.Instantiate(markerPrefab, position, Quaternion.identity);
        marker.gameObject.layer = config.LayerCodeToIndex(layerCode);
        markersRegistry[nextId] = marker;
        markerToId[marker.gameObject] = nextId;
        return nextId;
    }

    public void UnregisterMarker(RaycastId id) {
        var marker = markersRegistry[id];
        markerToId.Remove(marker.gameObject);
        markersRegistry.Remove(id);
        UnityEngine.Object.Destroy(marker.gameObject);
    }

    public void UpdateMarker(RaycastId id, Vector3 position) {
        var marker = markersRegistry[id];
        marker.transform.position = position;
    }

    public RaycastState ReadState(RaycastId raycastId) {
        return new RaycastState {
            radius = markersRegistry[raycastId].Radius
        };
    }

    // todo: can also be configured to register static collision obstacles in one raycast?
    public bool Raycast(Ray ray, float maxDistance, ReservedLayerCode layerCode, out RaycastId metadata, out RaycastHit hitInfo) {
        if (Physics.Raycast(ray, out hitInfo, maxDistance, 1 << config.LayerCodeToIndex(layerCode))) {
            metadata = markerToId[hitInfo.collider.gameObject];
            return true;
        } else {
            metadata = default;
            return false;
        }
    }

    public int Overlap(Vector3 position, float radius, ReservedLayerCode layerCode, out List<RaycastId> idsResult) {
        idsResultBuffer.Clear();
        int overlapCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, config.LayerCodeToMask(layerCode));
        for (int i = 0; i < overlapCount; i++) {
            idsResultBuffer.Add(markerToId[overlapBuffer[i].gameObject]);
        }

        idsResult = idsResultBuffer;
        return overlapCount;
    }

}