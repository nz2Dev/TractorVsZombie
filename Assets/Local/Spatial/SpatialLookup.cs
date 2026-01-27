using KNN;
using KNN.Jobs;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

public interface IPositionSource {
    public Vector3 Position { get; }
}

public class SpatialLookup<T> where T : IPositionSource {
    
    private T[] sources;
    private int sourceCount;
    private NativeArray<float3> points;
    private KnnContainer knnContainer;

    private NativeArray<int> resultBuffer;

    public SpatialLookup(int initSize) {
        sources = new T[initSize];
        points = new NativeArray<float3>(initSize, Allocator.Persistent);
        knnContainer = new KnnContainer(points.GetSubArray(0, 1), false, Allocator.TempJob);
        resultBuffer = new NativeArray<int>(1, Allocator.Persistent);
    }

    public int SourceCount => sourceCount;

    public void Reset() {
        sourceCount = 0;
    }

    public void Add(T source) {
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

}