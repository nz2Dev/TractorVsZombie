using System;
using System.Collections.Generic;

using UnityEngine;

public class ProximityService {

    internal class SolverLayer {
        
        private readonly KnnSolver solver;
        private readonly Dictionary<int, int> metadataToBeaconId = new ();
        private readonly Dictionary<int, int> beaconIdToMetadata = new ();

        public SolverLayer(KnnSolver solver) {
            this.solver = solver;
        }

        internal void RegisterPoint(int metadata, Vector3 position) {
            var pointId = solver.AddPoint(position);
            metadataToBeaconId[metadata] = pointId;
            beaconIdToMetadata[pointId] = metadata;
        }

        internal void UpdatePoint(int metadata, Vector3 position) {
            var pointId = metadataToBeaconId[metadata];
            solver.UpdatePoint(pointId, position);
        }

        internal void RemovePoint(int metadata) {
            var pointId = metadataToBeaconId[metadata];
            solver.RemovePoint(pointId);
            metadataToBeaconId.Remove(metadata);
            beaconIdToMetadata.Remove(pointId);
        }

        internal bool QueryNearest(Vector3 position, out int metadata) {
            var beaconId = solver.QueryNearest(position);
            if (beaconId != -1) {
                metadata = beaconIdToMetadata[beaconId];
                return true;
            } else {
                metadata = -1;
                return false;
            }
        }
    }

    private readonly KnnRunner knnRunner;

    private Dictionary<int, SolverLayer> layers = new();
    private int layerIdCounter = 0;

    public ProximityService(KnnRunner knnRunner) {
        this.knnRunner = knnRunner;
    }

    public int CreateLayer() {
        var nextLayerId = ++layerIdCounter;
        layers[nextLayerId] = new SolverLayer(knnRunner.CreateSolver());
        return nextLayerId;
    }

    public void RegisterPoint(Vector3 position, int metadata, int layerId) {
        layers[layerId].RegisterPoint(metadata, position);
    }

    public void UpdatePoint(int metadata, Vector3 position, int layerId) {
        layers[layerId].UpdatePoint(metadata, position);
    }

    public void RemoveBeacon(int metadata, int layerId) {
        layers[layerId].RemovePoint(metadata);
    }

    public bool QueryNearestBeacon(Vector3 position, out int metadata, int layerId) {
        return layers[layerId].QueryNearest(position, out metadata);
    }

}