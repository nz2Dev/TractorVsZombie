using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public struct Wall {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 size;
}

public class LevelProvider : MonoBehaviour {
    
    [SerializeField] private LayerMask wallsMask;

    private List<Wall> walls;

    private void Start() {
        walls = new List<Wall>(32);
        var colliders = FindObjectsOfType<BoxCollider>(includeInactive: true);
        foreach (var collider in colliders) {
            if (MaskContainsLayer(wallsMask, collider.gameObject.layer)) {
                var scaledBoxSize = collider.size;
                scaledBoxSize.Scale(collider.transform.lossyScale);
                walls.Add(new Wall {
                    position = collider.transform.position,
                    rotation = collider.transform.rotation,
                    size = scaledBoxSize
                });
            }
        }
    }

    private static bool MaskContainsLayer(LayerMask layerMask, int layer) {
        return (layerMask.value & (1 << layer)) != 0;
    }

    public IEnumerable<Wall> GetWalls() {
        return walls;
    }

}