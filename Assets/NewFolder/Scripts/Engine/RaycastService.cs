using System.Collections.Generic;

using UnityEngine;

public enum ReservedLayerCode {
    First,
    Second
}

public class RaycastService {
    
    private readonly Dictionary<int, GameObject> markersRegistry = new();
    private readonly Dictionary<GameObject, int> markerToId = new();

    private readonly RaycastConfig config;
    private readonly Collider[] overlapBuffer;
    private readonly List<int> idsResultBuffer;

    private int idCounter;

    public RaycastService(RaycastConfig config) {
        this.config = config;
        overlapBuffer = new Collider[config.overlapBufferSize];
        idsResultBuffer = new (config.overlapBufferSize);
    }

    public int RegisterMarker(Vector3 position, RaycastMarker markerPrefab, ReservedLayerCode layerCode) {
        var nextId = ++idCounter;
        var marker = GameObject.Instantiate(markerPrefab, position, Quaternion.identity);
        marker.gameObject.layer = config.LayerCodeToIndex(layerCode);
        markersRegistry[nextId] = marker.gameObject;
        markerToId[marker.gameObject] = nextId;
        return nextId;
    }

    public void UnregisterMarker(int id) {
        var marker = markersRegistry[id];
        markerToId.Remove(marker);
        markersRegistry.Remove(id);
        UnityEngine.Object.Destroy(marker);
    }

    public void UpdateMarker(int id, Vector3 position) {
        var marker = markersRegistry[id];
        marker.transform.position = position;
    }

    public bool Raycast(Ray ray, float maxDistance, ReservedLayerCode layerCode, out int metadata, out RaycastHit hitInfo) {
        if (Physics.Raycast(ray, out hitInfo, maxDistance, 1 << config.LayerCodeToIndex(layerCode))) {
            metadata = markerToId[hitInfo.collider.gameObject];
            return true;
        } else {
            metadata = -1;
            return false;
        }
    }

    public int Overlap(Vector3 position, float radius, ReservedLayerCode layerCode, out List<int> idsResult) {
        idsResultBuffer.Clear();
        int overlapCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, config.LayerCodeToMask(layerCode));
        for (int i = 0; i < overlapCount; i++) {
            idsResultBuffer.Add(markerToId[overlapBuffer[i].gameObject]);
        }

        idsResult = idsResultBuffer;
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