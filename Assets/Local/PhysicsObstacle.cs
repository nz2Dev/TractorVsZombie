using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PhysicsObstacle : MonoBehaviour {
    public Vector3 bakedSize;

    private void OnValidate() {
        bakedSize = GetComponent<BoxCollider>().size;
    }
}
