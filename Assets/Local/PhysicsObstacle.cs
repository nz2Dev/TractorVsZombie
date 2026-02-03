using UnityEngine;

// Is used as a hack to get a shape editor.
// But PhysicsService and VehicleService use them as natural UnityPhysics collider as it is. Be careful!
[RequireComponent(typeof(BoxCollider))]
public class PhysicsObstacle : MonoBehaviour {
    public Vector3 bakedSize;

    private void OnValidate() {
        bakedSize = GetComponent<BoxCollider>().size;
    }
}
