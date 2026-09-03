using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Unity.Mathematics;

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ORCAMeshObstacleVertices : ORCAObstacleVertices {
    
    [HideInInspector, SerializeField] private MeshFilter meshFilter;
    
    private float3[] localVertices;
    
    public override bool InverseORCAOrder => false;

#if UNITY_EDITOR
    private void OnValidate() {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
            ComputeLocalVertices();
    }
#endif

    private void Awake() {
        Assert.NotNull(meshFilter);
        ComputeLocalVertices();
    }

    private void ComputeLocalVertices() {
        localVertices = MeshTo2DPolygon.ExtractXZHull(meshFilter.sharedMesh)
            .ToArray();
    }
    
    public override void ReadWorldVertices(List<float3> verticesBuffer) {
        verticesBuffer.Clear();
        for (int i = 0; i < localVertices.Length; i++)
            verticesBuffer.Add(transform.TransformPoint(localVertices[i]));
    }

}