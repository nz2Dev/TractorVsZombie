using System.Collections.Generic;

using UnityEngine;

public interface IMetadata {
    public int Id { get; }
}

public class CollisionLookup<T> where T : IMetadata {
    
    private int layer;
    private Dictionary<Collider, T> colliderToMetadata = new();
    private Dictionary<int, Collider> metadataToCollider = new();
    private Collider[] resultsBuffer;
    private T[] resultsMetadataBuffer;

    public CollisionLookup(int layer, int resultsCapacity) {
        this.layer = layer;
        resultsBuffer = new Collider[resultsCapacity];
        resultsMetadataBuffer = new T[resultsCapacity];
    }

    public void Add(T metadata, Vector3 position, float height, float radius) {
        var marker = CreateMarker(metadata.Id, position, height, radius);
        colliderToMetadata[marker] = metadata;
        metadataToCollider[metadata.Id] = marker;
    }

    public void Update(T metadata, Vector3 position) {
        metadataToCollider[metadata.Id].transform.position = position;
    }

    public void Remove(T metadata) {
        metadataToCollider.Remove(metadata.Id, out var marker);
        colliderToMetadata.Remove(marker);
        GameObject.Destroy(marker.gameObject);
    }

    public bool Raycast(Vector3 origin, Vector3 direction, float distance, out T hitMetadata) {
        var result = Physics.Raycast(origin, direction, out var hitInfo, distance, 1 << layer);
        hitMetadata = default;
        if (result) hitMetadata = colliderToMetadata[hitInfo.collider];
        return result;
    }

    public int Overlap(Vector3 position, float radius, out T[] results) {
        var overlapCount = Physics.OverlapSphereNonAlloc(position, radius, resultsBuffer, 1 << layer);
        for (int i = 0; i < overlapCount; i++) {
            var overlappedCollider = resultsBuffer[i];
            resultsMetadataBuffer[i] = colliderToMetadata[overlappedCollider];
        }
        results = resultsMetadataBuffer;
        return overlapCount;
    }

    private CapsuleCollider CreateMarker(int id, Vector3 position, float height, float radius) {
        var go = new GameObject("Collision Marker (New) " + id, typeof(CapsuleCollider));
        go.transform.position = position;
        go.layer = layer;
        var collider = go.GetComponent<CapsuleCollider>();
        collider.isTrigger = true;
        collider.height = height;
        collider.radius = radius;
        collider.center = new Vector3(0, height * 0.5f, 0);
        return collider;
    }

}