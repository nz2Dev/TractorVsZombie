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
    
    internal int Count => pointsRegistry.Count;

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
        var pointId = indexToId[index];
        if (pointsRegistry.ContainsKey(pointId)) {
            return pointId;
        } else {
            return -1;
        }
    }

    public void RemovePoint(int id) {
        pointsRegistry.Remove(id);
        // as removing point from registry do not invalidate the built points
        // this alone can't prevent GetIndexId(int) return next valid id from the built points.
        // the solution is in Solver, that query K nearest, and increase the K, until it find first valid index that successfully maps to the point id here.
    }

    public void Dispose() {
        pointsBuffer.Dispose();
    }

}