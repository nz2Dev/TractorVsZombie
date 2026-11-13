using UnityEngine;

public class WeaponOutline : MonoBehaviour {
    
    public Vector3 AimPoint {
        set {
            var aimYFlat = new Vector3(value.x, 0, value.z);
            var pos = transform.position;
            var posYFlat = new Vector3(pos.x, 0, pos.z);
            var posToAim = aimYFlat - posYFlat;
            transform.rotation = Quaternion.LookRotation(posToAim.normalized, Vector3.up);
        }
    }

    public Vector3 Position {
        set {
            transform.position = new Vector3(value.x, 0, value.z);
        }
    }

    private void OnValidate() {
        AimPoint = transform.position + Vector3.forward;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
    
}