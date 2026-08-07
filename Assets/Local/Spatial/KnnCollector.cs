using System;
using System.Collections.Generic;

using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;

public class KnnCollector : IDisposable {
    
    private NativeArray<float3> pointsBuffer;

    private int idCounter;
    private Dictionary<int, float3> pointsRegistry = new ();
    private Dictionary<int, int> indexToId = new();

    public KnnCollector(int capacity) {
        pointsBuffer = new NativeArray<float3>(capacity, Allocator.Persistent);
    }

    public NativeArray<float3> BuildPoints() {
        var assignIndex = 0;
        foreach (var id in pointsRegistry.Keys) {
            pointsBuffer[assignIndex] = pointsRegistry[id];
            indexToId[assignIndex] = id;
            assignIndex++;
        }
        return pointsBuffer.GetSubArray(0, pointsRegistry.Count);
    }

    public void Clear() {
        pointsRegistry.Clear();
    }

    public int AddPoint(float3 point) {
        var nextId = ++idCounter;
        pointsRegistry[nextId] = point;
        return nextId;
    }

    public void UpdatePoint(int id, float3 point) {
        pointsRegistry[id] = point;
    }

    public int GetIndexId(int index) {
        return indexToId[index];
    }

    public void RemovePoint(int id) {
        pointsRegistry.Remove(id);
        indexToId.Remove(id);
    }

    public void Dispose() {
        pointsBuffer.Dispose();
    }

}