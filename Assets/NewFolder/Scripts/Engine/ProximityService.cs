using System;
using System.Collections.Generic;

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

    public int AddPoint(Vector3 position, Layer layer) {
        return knnRunner.System.AddPoint(position, (int) layer);
    }

    public void UpdatePoint(int id, Vector3 position) {
        knnRunner.System.UpdatePoint(id, position);
    }

    public void RemovePoint(int metadata) {
        knnRunner.System.RemovePoint(metadata);
    }

    public bool QueryNearestPoint(Vector3 position, Layer layer, out int id) {
        id = knnRunner.System.QueryNearest(position, (int) layer);
        return id != -1;
    }

}