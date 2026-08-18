using System;
using System.Collections.Generic;

using Unity.Collections;
using Unity.Mathematics;

public class KnnSystem : IDisposable {
    
    private int idCounter;
    private readonly List<KnnLayer> layers;
    private readonly Dictionary<int, int> idToLayer;

    private NativeArray<int> resultBuffer;
    private NativeList<int> variableResultBuffer;

    public KnnSystem(int intiResultCapacity, int intiSizeCapacity, int layersCount) {
        layers = new ();
        idToLayer = new ();
        resultBuffer = new NativeArray<int>(1, Allocator.Persistent);
        variableResultBuffer = new NativeList<int>(intiResultCapacity, Allocator.Persistent);
        for (int i = 0; i < layersCount; i++) {
            layers.Add(new KnnLayer(intiSizeCapacity));
        }
    }

    public int AddPoint(float3 point, int layerIndex) {
        var nextId = idCounter++;
        var layer = layers[layerIndex];
        layer.AddPoint(nextId, point);
        idToLayer[nextId] = layerIndex;
        return nextId;
    }

    public void UpdatePoint(int id, float3 point) {
        var layerIndex = idToLayer[id];
        var layer = layers[layerIndex];
        layer.UpdatePoint(id, point);
    }

    public void RemovePoint(int id) {
        var layerIndex = idToLayer[id];
        var layer = layers[layerIndex];
        layer.RemovePoint(id);
        idToLayer.Remove(id);
    }

    public int QueryNearest(float3 position, int layerIndex) {
        var layer = layers[layerIndex];
        if (!layer.Container.HasValue)
            return -1;
        
        layer.Container.Value.QueryKNearest(position, resultBuffer);
        return layer.GetIndexId(resultBuffer[0]);
    }

    public void Update() {
        foreach (var layer in layers) {
            layer.Build();
        }
    }

    public void Dispose() {
        foreach (var layer in layers) {
            layer.Dispose();
        }
        
        layers.Clear();
        resultBuffer.Dispose();
        variableResultBuffer.Dispose();
    }
}