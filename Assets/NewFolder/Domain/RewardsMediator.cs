using System.Collections.Generic;

using UnityEngine;

public class RewardsMediator {

    private class SpatialPoint {
        public int id;
        public SphereCollider marker;
        public int spawnFrame;
    }
    
    private readonly int rewardLayer;
    private readonly LayerMask rewardsMask;

    private int rewardIdCounter;
    private readonly List<SpatialPoint> points = new ();
    private readonly Dictionary<Collider, SpatialPoint> colliderToPoint = new ();
    private readonly Collider[] overlapBuffer = new Collider[512];

    public RewardsMediator(int markersLayer) {
        this.rewardLayer = markersLayer;
        rewardsMask = 1 << markersLayer;
    }

    public int AddRewardPoint(Vector3 position, float radius) {
        int nextRewardId = rewardIdCounter++;
        
        var marker = CreateColliderMarker(nextRewardId, position, radius);
        var spatialPoint = new SpatialPoint {
            id = nextRewardId, 
            marker = marker, 
            spawnFrame = Time.frameCount,
        };
        
        points.Add(spatialPoint);
        colliderToPoint[marker] = spatialPoint;
        return spatialPoint.id;
    }

    public bool CollectRewardsPoints(Vector3 position, float radius, List<int> spatialPointIds) {
        var pointsCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, rewardsMask);
        
        spatialPointIds.Clear();
        for (int i = 0; i < pointsCount; i++) {
            var overlappedCollider = overlapBuffer[i];
            if (colliderToPoint.TryGetValue(overlappedCollider, out var point) && point.spawnFrame != Time.frameCount) {
                spatialPointIds.Add(point.id);
                RemoveReward(point);
            }
        }
        
        return spatialPointIds.Count > 0;
    }

    private void RemoveReward(SpatialPoint reward) {
        colliderToPoint.Remove(reward.marker);
        Object.Destroy(reward.marker.gameObject);
        points.Remove(reward);
    }

    private SphereCollider CreateColliderMarker(int nextRewardId, Vector3 position, float radius) {
        var gameObject = new GameObject("reward " + nextRewardId, typeof(SphereCollider));
        gameObject.layer = rewardLayer;
        gameObject.transform.position = position;
        var collider = gameObject.GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = radius;
        return collider;
    }

}