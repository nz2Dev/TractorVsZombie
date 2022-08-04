using UnityEngine;

public class GrenaderController : MonoBehaviour {

    [SerializeField] private Ammo ammo;
    [SerializeField] private Grenader grenader;
    [SerializeField] private LayerMask groundLayerMask;

    public void OnPointerDown() {
        if (ammo.RequestAmmo()) {
            var camera = Camera.main;
            var clickRay = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out var hitInfo, float.MaxValue, groundLayerMask)) {
                grenader.Aim(hitInfo.point);
            }
        }
    }

    public void OnPointerStay() {
        if (ammo.RequestAmmo()) {
            var camera = Camera.main;
            var clickRay = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out var hitInfo, float.MaxValue, groundLayerMask)) {
                grenader.Aim(hitInfo.point);
            }
        }
    }

    public void OnPointerUp() {
        if (grenader.IsAimed) {
            if (ammo.TakeAmmo()) {
                grenader.FireLastAimPoint();
            }
        }
    }

}