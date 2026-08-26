using UnityEngine;

public class KnnRunner : MonoBehaviour {
    
    [SerializeField] private int intiSizeCapacity = 256;
    [SerializeField] private int intiResultCapacity = 256;
    [SerializeField] private int layersCount = 3;

    public KnnSystem System { get; private set; }

    private void Awake() {
        System = new KnnSystem(intiResultCapacity, intiSizeCapacity, layersCount);
    }

    private void Update() {
        System.Update();
    }

    private void OnDestroy() {
        System.Dispose();
    }

}