using System;
using System.Collections.Generic;

using UnityEngine;

public class ProximityService {

    public enum Layer {
        CombatReservedA,
        CombatReservedB
    }

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

    private readonly List<SolverLayer> layers = new();

    public ProximityService(KnnRunner knnRunner) {
        this.knnRunner = knnRunner;
        // still requires the central object that holds and manages this, but we will keep it this simple for now
        // NOTE: instantiating ProximityService twise, will recreate layers
        CreateLayers();
    }

    private void CreateLayers() {
        var values = Enum.GetValues(typeof(Layer));
        for (int i = 0; i < values.Length; i++) {
            layers.Add(new SolverLayer(knnRunner.CreateSolver()));
        }
    }

    public void RegisterPoint(Vector3 position, int metadata, Layer layer) {
        layers[(int) layer].RegisterPoint(metadata, position);
    }

    public void UpdatePoint(int metadata, Vector3 position, Layer layer) {
        layers[(int) layer].UpdatePoint(metadata, position);
    }

    public void RemoveBeacon(int metadata, Layer layer) {
        layers[(int) layer].RemovePoint(metadata);
    }

    public bool QueryNearestBeacon(Vector3 position, out int metadata, Layer layer) {
        return layers[(int) layer].QueryNearest(position, out metadata);
    }

}