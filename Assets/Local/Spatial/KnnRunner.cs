using KNN;
using KNN.Jobs;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

public class KnnRunner : MonoBehaviour {
    
    [SerializeField] private int intiSizeCapacity = 256;
    [SerializeField] private int intiResultCapacity = 256;

    private KnnSolver knnSolver;
    public KnnSolver Solver => knnSolver;

    private void Awake() {
        knnSolver = new KnnSolver(intiSizeCapacity, intiResultCapacity);
    }

    void FixedUpdate() {
        knnSolver.Solve();
    }

    private void OnDestroy() {
        knnSolver.Dispose();
    }

}