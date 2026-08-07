using KNN;
using KNN.Jobs;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

public class KnnRunner : MonoBehaviour {
    
    [SerializeField] private int intiSizeCapacity = 256;

    private KnnContainer container;
    private KnnCollector collector;
    
    private NativeArray<int> resultBuffer;
    private NativeList<int> variableResultBuffer;

    private void Awake() {
        collector = new KnnCollector(intiSizeCapacity);
        container = new KnnContainer(collector.BuildPoints(), false, Allocator.TempJob);
        resultBuffer = new NativeArray<int>(1, Allocator.Persistent);
        variableResultBuffer = new NativeList<int>(256, Allocator.Persistent);
    }

    void FixedUpdate() {
        container.Dispose();
        container = new KnnContainer(collector.BuildPoints(), false, Allocator.TempJob);
        new KnnRebuildJob(container).Schedule().Complete();
    }

    private void OnDestroy() {
        collector.Dispose();
        container.Dispose();
        resultBuffer.Dispose();
        variableResultBuffer.Dispose();
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

    public int QueryNearest(Vector3 queryPosition) {
        container.QueryKNearest(queryPosition, resultBuffer.GetSubArray(0, 1));
        return collector.GetIndexId(resultBuffer[0]);
    }

}