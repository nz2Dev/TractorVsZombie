using System;

using AiEditorToolsSdk.Components.Organization.Responses;

using KNN;
using KNN.Jobs;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public class KnnSolver : IDisposable {

    private KnnContainer? container;
    private readonly KnnCollector collector;

    private NativeArray<int> resultBuffer;
    private NativeList<int> variableResultBuffer;

    public KnnSolver(int intiSizeCapacity, int resultCapacity) {
        collector = new KnnCollector(intiSizeCapacity);
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

    internal void Solve() {
        if (collector.Count == 0) {
            return;
        }

        if (container.HasValue) {
            container.Value.Dispose();
        }

        var points = collector.BuildPoints();
        container = new KnnContainer(points, buildNow: true, Allocator.TempJob);
    }

    public int QueryNearest(float3 position) {
        if (!container.HasValue)
            return -1;
        
        container.Value.QueryKNearest(position, resultBuffer);
        return collector.GetIndexId(resultBuffer[0]);
    }

    public void Dispose() {
        collector.Dispose();
        container?.Dispose();
        resultBuffer.Dispose();
        variableResultBuffer.Dispose();
    }

}