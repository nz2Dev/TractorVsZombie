using System;

using KNN;
using KNN.Jobs;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public class KnnSolver : IDisposable {

    private KnnContainer container;
    private readonly KnnCollector collector;

    private NativeArray<int> resultBuffer;
    private NativeList<int> variableResultBuffer;

    public KnnSolver(int intiSizeCapacity, int resultCapacity) {
        collector = new KnnCollector(intiSizeCapacity);
        container = new KnnContainer(collector.BuildPoints(), false, Allocator.TempJob);

        resultBuffer = new NativeArray<int>(1, Allocator.Persistent);
        variableResultBuffer = new NativeList<int>(resultCapacity, Allocator.Persistent);
    }

    public int AddPoint(float3 point) {
        return collector.AddPoint(point);
    }

    public void UpdatePoint(int id, float3 point) {
        collector.UpdatePoint(id, point);
    }

    public void RemovePoint(int id) {
        collector.RemovePoint(id);
    }

    internal JobHandle ScheduleSolve() {
        container.Dispose();
        var points = collector.BuildPoints();
        container = new KnnContainer(points, false, Allocator.TempJob);
        return new KnnRebuildJob(container).Schedule();
    }

    public int QueryNearest(float3 position) {
        container.QueryKNearest(position, resultBuffer);
        return collector.GetIndexId(resultBuffer[0]);
    }

    public void Dispose() {
        collector.Dispose();
        container.Dispose();
        resultBuffer.Dispose();
        variableResultBuffer.Dispose();
    }
}