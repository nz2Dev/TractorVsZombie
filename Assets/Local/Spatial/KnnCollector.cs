using System;
using System.Collections.Generic;

using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;

public class KnnCollector : IDisposable {
    
    private NativeArray<float3> pointsBuffer;

    private int idCounter;
    private Dictionary<int, float3> pointsRegistry = new ();
    private Dictionary<int, int> idToIndex = new();

    public KnnCollector(int capacity) {
        pointsBuffer = new NativeArray<float3>(capacity, Allocator.Persistent);
    }

    public NativeArray<float3> BuildPoints() {
        var assignIndex = 0;
        foreach (var id in pointsRegistry.Keys) {
            pointsBuffer[assignIndex] = pointsRegistry[id];
            idToIndex[id] = assignIndex;
            assignIndex++;
        }
        return pointsBuffer.GetSubArray(0, Mathf.Max(pointsRegistry.Count, 1));
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
        return idToIndex[index];
    }

    public void RemovePoint(int id) {
        pointsRegistry.Remove(id);
        idToIndex.Remove(id);
    }

    public void Dispose() {
        pointsBuffer.Dispose();
    }

}