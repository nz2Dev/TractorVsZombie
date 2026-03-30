using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;

public abstract class BaseObstacleSource : MonoBehaviour {
    public abstract float3[] Vertices { get; }
}