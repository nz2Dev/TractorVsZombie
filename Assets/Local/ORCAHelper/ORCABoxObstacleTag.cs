using Mono.Cecil.Cil;

using Unity.Mathematics;

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class ORCABoxObstacleTag : MonoBehaviour {
    
    private BoxCollider boxCollider;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void GetBoxInfo(out Vector3 position, out Quaternion rotation, out Vector3 boxSize) {
        transform.GetPositionAndRotation(out position, out rotation);
        boxSize = boxCollider.size;
        boxSize.Scale(transform.lossyScale);
    }

}