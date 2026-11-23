using UnityEngine;

// Aims flat surface UP vector toward the Aim Point, and its forward vector always downwards.
public class FlatSurfaceRig : MonoBehaviour {

    [NonNull, SerializeField] private Transform surface;
    
    public Vector3 AimPoint {
        set {
            var aimYFlat = new Vector3(value.x, 0, value.z);
            var pos = transform.position;
            var posYFlat = new Vector3(pos.x, 0, pos.z);
            var posToAim = aimYFlat - posYFlat;
            surface.rotation = Quaternion.LookRotation(Vector3.down, posToAim);
        }
    }

    private void OnValidate() {
        AimPoint = transform.position + Vector3.forward;
    }

}