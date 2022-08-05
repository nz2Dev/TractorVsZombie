using UnityEngine;

public class GrenaderController : MonoBehaviour {

    [SerializeField] private Ammo ammo;
    [SerializeField] private Grenader grenader;

    public void AimGreander(Vector3 point) {
        if (ammo.RequestAmmo()) {
            grenader.Aim(point);
        }
    }

    public void FireGrenade() {
        if (grenader.IsAimed) {
            if (ammo.TakeAmmo()) {
                grenader.FireLastAimPoint();
            }
        }
    }

}