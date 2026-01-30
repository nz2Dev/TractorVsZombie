using System.Collections.Generic;

using KNN;
using KNN.Jobs;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

public interface IPositionSource {
    public Vector3 Position { get; }
}

// TODO: Implement IDisposable, native resources ARE NOT dealocated
public class SpatialLookup<T> where T : IPositionSource {
    
    private T[] sources;
    private int sourceCount;
    private NativeArray<float3> points;
    private KnnContainer knnContainer;

    private NativeArray<int> resultBuffer;
    private NativeList<int> variableResultBuffer;
    private List<T> resultSourcesBuffer;

    public SpatialLookup(int initSize, int initRangeCapacity = 256) {
        sources = new T[initSize];
        points = new NativeArray<float3>(initSize, Allocator.Persistent);
        knnContainer = new KnnContainer(points.GetSubArray(0, 1), false, Allocator.TempJob);
        resultBuffer = new NativeArray<int>(1, Allocator.Persistent);
        variableResultBuffer = new NativeList<int>(initRangeCapacity, Allocator.Persistent);
        resultSourcesBuffer = new List<T>(initRangeCapacity);
    }

    public int SourceCount => sourceCount;

    public void Reset() {
        sourceCount = 0;
    }

    public void Add(T source) {
        // TODO: sources.Length is tested here. Use list?
        if (sourceCount >= sources.Length)
            return;

        sources[sourceCount++] = source;
    }

    public void Fixate() {
        if (sourceCount > points.Length) {
            points.Dispose();
            points = new NativeArray<float3>(sourceCount * 2, Allocator.Persistent);
        }

        for (int i = 0; i < sourceCount; i++)
            points[i] = sources[i].Position;
    }

    public JobHandle ScheduleBuild() {
        knnContainer.Dispose();
        knnContainer = new KnnContainer(points.GetSubArray(0, sourceCount), false, Allocator.TempJob);
        return new KnnRebuildJob(knnContainer).Schedule();
    }

    public T QueryNearest(Vector3 queryPosition) {
        knnContainer.QueryKNearest(queryPosition, resultBuffer.GetSubArray(0, 1));
        return sources[resultBuffer[0]];
    }

    public List<T> QueryRange(Vector3 queryPosition, float radius) {
        resultSourcesBuffer.Clear();
        if (sourceCount == 0) {
            return resultSourcesBuffer;
        }

        variableResultBuffer.Clear();
        knnContainer.QueryRange(queryPosition, radius, variableResultBuffer);
        for (int i = 0; i < variableResultBuffer.Length; i++) {
            var indexInRadius = variableResultBuffer[i];
            resultSourcesBuffer.Add(sources[indexInRadius]);
        }
        return resultSourcesBuffer;
    }

}