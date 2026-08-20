using System;

using KNN;
using KNN.Jobs;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

internal class KnnLayer : IDisposable {

    private KnnContainer? container;
    private readonly KnnCollector collector;

    internal KnnContainer? Container => container;

    public KnnLayer(int intiSizeCapacity) {
        collector = new KnnCollector(intiSizeCapacity);
    }

    public void AddPoint(int id, float3 point) {
        collector.AddPoint(id, point);
    }

    public void UpdatePoint(int id, float3 point) {
        collector.UpdatePoint(id, point);
    }

    public Vector3 GetPoint(int id) {
        return collector.GetPoint(id);
    }

    public void RemovePoint(int id) {
        collector.RemovePoint(id);
    }

    internal int GetIndexId(int index) {
        return collector.GetIndexId(index);
    }

    internal void Build() {
        if (collector.Count == 0) {
            return;
        }

        if (container.HasValue) {
            container.Value.Dispose();
        }

        var points = collector.BuildPoints();
        container = new KnnContainer(points, buildNow: true, Allocator.TempJob);
    }

    public void Dispose() {
        collector.Dispose();
        container?.Dispose();
    }

}