using System;
using System.Collections.Generic;

using UnityEngine;

public enum ReservedLayerCode {
    First,
    Second
}

public class PhysicsManager : MonoBehaviour {

    [SerializeField] private int overlapBufferSize = 128;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask environmentMask;
    [Space]
    [SerializeField] private int firstReservedRaycastLayer;
    [SerializeField] private int secondReservedRaycastLayer;

    private Collider[] overlapBuffer;

    public int OverlapBufferSize => overlapBufferSize;

    private void Awake() {
        overlapBuffer = new Collider[overlapBufferSize];
    }

    public PhysicsBody InstantiateBody(PhysicsBody bodyPrefab, Vector3 position, Quaternion rotation) {
        var instance = GameObject.Instantiate(bodyPrefab, position, rotation);
        return instance;
    }
    
    internal void DestroyBody(PhysicsBody body) {
        UnityEngine.Object.Destroy(body.gameObject);
    }

    internal PhysicsObstacleNew InstantiateObstacle(PhysicsObstacleNew obstaclePrefab, Vector3 position, Quaternion rotation) {
        var instance = GameObject.Instantiate(obstaclePrefab, position, rotation);
        return instance;
    }

    internal void DestroyObstacle(PhysicsObstacleNew obstacle) {
        UnityEngine.Object.Destroy(obstacle.gameObject);
    }

    public bool RaycastGround(Ray ray, float maxDistance, out RaycastHit hitInfo) {
        return Physics.Raycast(ray, out hitInfo, maxDistance, groundMask);
    }

    internal bool RaycastEnvironment(Ray ray, float maxDistance, out RaycastHit hitInfo) {
        return Physics.Raycast(ray, out hitInfo, maxDistance, environmentMask);
    }

    public CollisionMarker InstantiateReservedRaycastMarker(CollisionMarker markerPrefab, Vector3 position, ReservedLayerCode layerCode) {
        var markerInstance = GameObject.Instantiate(markerPrefab, position, Quaternion.identity);
        markerInstance.gameObject.layer = ResolveLayerIndex(layerCode);
        return markerInstance;
    }

    public void DestroyReservedRaycastMarker(CollisionMarker marker) {
        UnityEngine.Object.Destroy(marker.gameObject);
    }

    public bool RaycastReservedMarkers(Ray ray, out RaycastHit hitInfo, float maxDistance, ReservedLayerCode layerCode) {
        return Physics.Raycast(ray, out hitInfo, maxDistance, 1 << ResolveLayerIndex(layerCode));
    }

    public int OverlapReservedMarkers(Vector3 position, float radius, out  Collider[] resultsBuffer, ReservedLayerCode layerCode) {
        int overlapCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, 1 << ResolveLayerIndex(layerCode));
        resultsBuffer = overlapBuffer;
        return overlapCount;
    }

    private int ResolveLayerIndex(ReservedLayerCode layerCode) {
        if (layerCode == ReservedLayerCode.First) {
            return firstReservedRaycastLayer;
        } else if (layerCode == ReservedLayerCode.Second) {
            return secondReservedRaycastLayer;
        } else {
            throw new Exception();
        }
    } 
}