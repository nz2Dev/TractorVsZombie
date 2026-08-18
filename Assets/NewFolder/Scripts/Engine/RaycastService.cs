using System.Collections.Generic;

using UnityEngine;

public enum ReservedLayerCode {
    First,
    Second
}

public class RaycastService {
    
    private readonly Dictionary<int, GameObject> metadataToMarker = new();
    private readonly Dictionary<GameObject, int> markerToMetadata = new();
    private readonly List<int> metadataOverlapResultBuffer;

    private readonly RaycastConfig config;
    private readonly Collider[] overlapBuffer;

    public RaycastService(RaycastConfig config) {
        this.config = config;
        overlapBuffer = new Collider[config.overlapBufferSize];
        metadataOverlapResultBuffer = new (config.overlapBufferSize);
    }

    public void RegisterMarker(int metadata, Vector3 position, CollisionMarker markerPrefab, ReservedLayerCode layerCode) {
        var marker = GameObject.Instantiate(markerPrefab, position, Quaternion.identity);
        marker.gameObject.layer = config.LayerCodeToIndex(layerCode);
        metadataToMarker[metadata] = marker.gameObject;
        markerToMetadata[marker.gameObject] = metadata;
    }

    public void UnregisterMarker(int metadata) {
        var marker = metadataToMarker[metadata];
        markerToMetadata.Remove(marker);
        metadataToMarker.Remove(metadata);
        UnityEngine.Object.Destroy(marker);
    }

    public void UpdateMarker(int metadata, Vector3 position) {
        var marker = metadataToMarker[metadata];
        marker.transform.position = position;
    }

    public bool Raycast(Ray ray, float maxDistance, ReservedLayerCode layerCode, out int metadata, out RaycastHit hitInfo) {
        if (Physics.Raycast(ray, out hitInfo, maxDistance, 1 << config.LayerCodeToIndex(layerCode))) {
            metadata = markerToMetadata[hitInfo.collider.gameObject];
            return true;
        } else {
            metadata = -1;
            return false;
        }
    }

    public int Overlap(Vector3 position, float radius, ReservedLayerCode layerCode, out List<int> metadataResultBuffer) {
        int overlapCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, config.LayerCodeToMask(layerCode));
        metadataOverlapResultBuffer.Clear();
        for (int i = 0; i < overlapCount; i++) {
            metadataOverlapResultBuffer.Add(markerToMetadata[overlapBuffer[i].gameObject]);
        }
        metadataResultBuffer = metadataOverlapResultBuffer;
        return overlapCount;
    }

    public Vector3 GetClosestVerticalGroundPoint(Vector3 position) {
        if (Physics.Raycast(new Ray(position + Vector3.up, Vector3.down), out var hitInfo, maxDistance: 100, config.groundMask)) {
            return hitInfo.point;
        } else {
            return Vector3.zero;
        }
    }

    public Vector3 GetGroundHitPosition(Ray ray) {
        if (Physics.Raycast(ray, out var hitInfo, maxDistance: 1000, config.groundMask)) {
            return hitInfo.point;
        } else {
            return Vector3.zero;
        }
    }

    public bool RaycastEnvironment(Ray ray, float maxDistance, out RaycastHit hitInfo) {
        return Physics.Raycast(ray, out hitInfo,  maxDistance, config.environmentMask);
    }

}