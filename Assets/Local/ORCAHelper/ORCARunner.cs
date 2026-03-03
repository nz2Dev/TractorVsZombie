using UnityEngine;
using UnityEngine.Assertions;

public class ORCARunner : MonoBehaviour {

    private static ORCARunner _instance;

    private ORCASystem system;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        ORCASystem.Instance = new ORCASystem();
        system = ORCASystem.Instance;
    }

    private void Start() {
        Assert.AreEqual(system, ORCASystem.Instance);
        system.Recreate();

        var environmnt = FindFirstObjectByType<ORCAEnvironment>();
        if (environmnt == null)
            return;
        
        foreach (var bakedData in environmnt.BakedData) {
            system.StaticObstacles.Add(ObstaclesConverter.ToFloat3Vertices(bakedData.vertices), bakedData.inverseOrder);
        }
    }

    private void Update() {
        Assert.AreEqual(system, ORCASystem.Instance);
        system.Tick(Time.deltaTime);
    }

    void OnDestroy() {
        if (_instance == this)
            _instance = null;

        ORCASystem.Instance?.Dispose();
        var wasStaticInstance = ORCASystem.Instance;
        var wasLocalInstance = system;
        ORCASystem.Instance = null;
        system = null;
        Assert.AreEqual(wasLocalInstance, wasStaticInstance);
    }

}