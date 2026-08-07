using System.Collections.Generic;

using UnityEngine;

public class ProximityService {

    private readonly KnnRunner knnRunner;

    private readonly Dictionary<int, int> metadataToBeaconId = new ();
    private readonly Dictionary<int, int> beaconIdToMetadata = new ();

    public ProximityService(KnnRunner knnRunner) {
        this.knnRunner = knnRunner;
    }

    public void AddBeacon(int metadata, Vector3 position) {
        var pointId = knnRunner.AddPoint(position);
        metadataToBeaconId[metadata] = pointId;
        beaconIdToMetadata[pointId] = metadata;
    }

    public void UpdateBeacon(int metadata, Vector3 position) {
        var pointId = metadataToBeaconId[metadata];
        knnRunner.UpdatePoint(pointId, position);
    }

    public void RemoveBeacon(int metadata) {
        var pointId = metadataToBeaconId[metadata];
        knnRunner.RemovePoint(pointId);
        metadataToBeaconId.Remove(metadata);
        beaconIdToMetadata.Remove(pointId);
    }

}