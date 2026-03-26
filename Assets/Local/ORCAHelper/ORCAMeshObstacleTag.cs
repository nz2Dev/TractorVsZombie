using UnityEngine;

public class ORCAMeshObstacleTag : MonoBehaviour {
    
    public Vector3[] GetShapeVertices() {
        return MeshTo2DPolygon.ExtractXZHull(GetComponent<MeshFilter>())
            .ToArray();
    }

}