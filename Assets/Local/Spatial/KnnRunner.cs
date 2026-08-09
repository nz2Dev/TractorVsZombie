using System.Collections.Generic;

using UnityEngine;

public class KnnRunner : MonoBehaviour {
    
    [SerializeField] private int intiSizeCapacity = 256;
    [SerializeField] private int intiResultCapacity = 256;

    private List<KnnSolver> solvers;

    private void Awake() {
        solvers = new ();
    }

    void Update() {
        foreach (var solver in solvers)
            solver.Solve();
    }

    private void OnDestroy() {
        foreach (var solver in solvers)
            solver.Dispose();
        
        solvers.Clear();
    }

    public KnnSolver CreateSolver() {
        var solver = new KnnSolver(intiSizeCapacity, intiResultCapacity);
        solvers.Add(solver);
        return solver;
    }

    public void DeleteSolver(KnnSolver solver) {
        solvers.Remove(solver);
    }

}