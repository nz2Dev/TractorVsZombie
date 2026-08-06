using System.Collections.Generic;

using UnityEngine;

public class RaycastService {
    
    private readonly PhysicsManager physicsManager;

    private readonly Dictionary<int, GameObject> metadataToMarker = new();
    private readonly Dictionary<GameObject, int> markerToMetadata = new();
    private readonly List<int> metadataOverlapResultBuffer;

    public RaycastService(PhysicsManager physicsManager) {
        this.physicsManager = physicsManager;
        metadataOverlapResultBuffer = new (physicsManager.OverlapBufferSize);
    }

    public void RegisterMarker(int metadata, Vector3 position, CollisionMarker markerPrefab, ReservedLayerCode layerCode) {
        var marker = physicsManager.InstantiateReservedRaycastMarker(markerPrefab, position, layerCode);
        metadataToMarker[metadata] = marker.gameObject;
        markerToMetadata[marker.gameObject] = metadata;
    }

    public void UnregisterMarker(int metadata) {
        var marker = metadataToMarker[metadata];
        markerToMetadata.Remove(marker);
        metadataToMarker.Remove(metadata);
        physicsManager.DestroyReservedRaycastMarker(marker.GetComponent<CollisionMarker>());
    }

    public void UpdateMarker(int metadata, Vector3 position) {
        var marker = metadataToMarker[metadata];
        marker.transform.position = position;
    }

    public bool Raycast(Ray ray, float maxDistance, ReservedLayerCode layerCode, out int metadata, out RaycastHit hitInfo) {
        if (physicsManager.RaycastReservedMarkers(ray, out hitInfo, maxDistance, layerCode)) {
            metadata = markerToMetadata[hitInfo.collider.gameObject];
            return true;
        } else {
            metadata = -1;
            return false;
        }
    }

    public int Overlap(Vector3 position, float radius, ReservedLayerCode layerCode, out List<int> metadataResultBuffer) {
        var overlapCount = physicsManager.OverlapReservedMarkers(position, radius, out var resultsBuffer, layerCode);
        metadataOverlapResultBuffer.Clear();
        for (int i = 0; i < overlapCount; i++) {
            metadataOverlapResultBuffer[i] = markerToMetadata[resultsBuffer[i].gameObject];
        }
        metadataResultBuffer = metadataOverlapResultBuffer;
        return overlapCount;
    }

}