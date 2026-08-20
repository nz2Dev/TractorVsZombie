using UnityEngine;

public class ProximityService {

    public enum Layer {
        CombatReservedA,
        CombatReservedB
    }

    private readonly KnnRunner knnRunner;

    public ProximityService(KnnRunner knnRunner) {
        this.knnRunner = knnRunner;
    }

    public ProximityId AddPoint(Vector3 position, Layer layer) {
        return new ProximityId(knnRunner.System.AddPoint(position, (int) layer));
    }

    public void UpdatePoint(ProximityId id, Vector3 position) {
        knnRunner.System.UpdatePoint(id.Value, position);
    }

    public Vector3 GetPoint(ProximityId id) {
        return knnRunner.System.GetPoint(id.Value);
    }

    public void RemovePoint(ProximityId id) {
        knnRunner.System.RemovePoint(id.Value);
    }

    public bool QueryNearestPoint(Vector3 position, Layer layer, out ProximityId id) {
        var pointId = knnRunner.System.QueryNearest(position, (int) layer);
        id = new ProximityId(pointId);
        return pointId != -1;
    }

}