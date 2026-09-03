using System;
using System.Collections.Generic;

using NUnit.Framework;

using Unity.Mathematics;

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ORCABoxObstacleVertices : ORCAObstacleVertices {
    
    [HideInInspector, SerializeField] private BoxCollider boxCollider;
    
    private float3[] localVertices;
    
    public override bool InverseORCAOrder => true;

#if UNITY_EDITOR
    private void OnValidate() {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
            ComputeLocalVertices();
    }
#endif

    private void Awake() {
        Assert.NotNull(boxCollider);
        ComputeLocalVertices();
    }

    public override void ReadWorldVertices(List<float3> verticesBuffer) {
        verticesBuffer.Clear();
        for (int i = 0; i < localVertices.Length; i++)
            verticesBuffer.Add(transform.TransformPoint(localVertices[i]));
    }

    private void ComputeLocalVertices() {
        localVertices = new float3[4];
        var boxSize = boxCollider.size;
        boxSize.Scale(transform.lossyScale);
        ComputeBoxVerticies(boxSize * 0.5f);
    }

    private void ComputeBoxVerticies(Vector3 halfSize) {
        var left = -halfSize.x;
        var right = halfSize.x;
        var forward = halfSize.z;
        var backward = -halfSize.z;
        localVertices[0] = new float3(left, 0, backward);
        localVertices[1] = new float3(left, 0, forward);
        localVertices[2] = new float3(right, 0, forward);
        localVertices[3] = new float3(right, 0, backward);
    }

}