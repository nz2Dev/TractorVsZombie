using System.Collections.Generic;
using System.Security;

using Mono.Cecil.Cil;

using Unity.Mathematics;

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class ORCABoxObstacleSource : BaseObstacleSource {
    
    private float3[] vertices;

    public override float3[] Vertices => vertices;

    private void Awake() {
        transform.GetPositionAndRotation(out var position, out var rotation);

        var boxCollider = GetComponent<BoxCollider>();
        var boxSize = boxCollider.size;
        boxSize.Scale(transform.lossyScale);

        vertices = ObstaclesConverter.ComputeBoxVerticies(position, rotation, boxSize * 0.5f);
    }

    private void Start() {
        if (Application.isPlaying)
            ORCASystem.Instance.AddObstacle(isStatic: true, inverseOrder: true, vertices);
    }

}