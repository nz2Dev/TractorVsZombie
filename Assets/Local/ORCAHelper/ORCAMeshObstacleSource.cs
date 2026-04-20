using System.Collections.Generic;
using System.Linq;

using Nebukam.ORCA;

using Unity.Mathematics;

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class ORCAMeshObstacleSource : BaseObstacleSource {

    private Obstacle obstacle;
    private float3[] vertices;

    public override float3[] Vertices => vertices;

    private void Awake() {
        var meshFilter = GetComponent<MeshFilter>();
        vertices = MeshTo2DPolygon.ExtractXZHull(meshFilter)
            .Select(vector => (float3) vector)
            .ToArray();
    }

    private void Start() {
        if (Application.isPlaying) {
            var orcaSystem = ORCASystem.Instance;
            obstacle = orcaSystem.AddObstacle(isStatic: true, inverseOrder: false, vertices);
        } 
    }

    private void OnDestroy() {
        if (Application.isPlaying) {
            ORCASystem.Instance.RemoveObstacle(obstacle);
        }
    }

}