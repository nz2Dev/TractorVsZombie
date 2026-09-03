using System.Collections.Generic;

using Unity.Mathematics;

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

        var verticesBuffer = new List<float3>();
        var obstacleVertices = FindObjectsByType<ORCAObstacleVertices>(FindObjectsSortMode.None);
        foreach (var vertices in obstacleVertices) {
            if (vertices.gameObject.isStatic) {
                vertices.ReadWorldVertices(verticesBuffer);
                system.AddObstacle(isStatic: true, vertices.InverseORCAOrder, verticesBuffer);
            }
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