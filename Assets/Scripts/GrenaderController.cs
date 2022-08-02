using UnityEngine;

public class GrenaderController : MonoBehaviour {
    
    [SerializeField] private Ammo ammo;
    [SerializeField] private Grenader grenader;
    [SerializeField] private LayerMask groundLayerMask;

    public void UpdateControl() {
        if (Input.GetMouseButton(0)) {
            if (ammo.RequestAmmo()) {
                var camera = Camera.main;
                var clickRay = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(clickRay, out var hitInfo, float.MaxValue, groundLayerMask)) {
                    grenader.Aim(hitInfo.point);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (grenader.IsAimed) {
                if (ammo.TakeAmmo()) {
                    grenader.FireLastAimPoint();
                }
            }
        }
    }

}