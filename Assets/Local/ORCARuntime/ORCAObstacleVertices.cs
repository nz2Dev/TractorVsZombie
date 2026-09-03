using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

/*
    Base class for vertices provider, for automatically adding vertices as obstacles, mark game object as static
*/
public abstract class ORCAObstacleVertices : MonoBehaviour {
    public abstract bool InverseORCAOrder { get; }
    public abstract void ReadWorldVertices(List<float3> vertexBuffer);
}